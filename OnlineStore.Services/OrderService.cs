using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.Order;
using System.Security.Cryptography;
using System.Text;
using static OnlineStore.Services.Common.ServiceConstants;

namespace OnlineStore.Services.Core
{
	public class OrderService : IOrderService
	{
		private readonly IOrderRepository _orderRepository;
		private readonly IShoppingCartRepository _shoppingCartRepository;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IRepository<OrderItem, int> _syncOrderItemRepository;
		private readonly IAsyncRepository<OrderItem, int> _asyncOrderItemRepository;
		private readonly ICheckoutRepository _checkoutRepository;
		private readonly IProductRepository _productRepository;

		public OrderService(IOrderRepository orderRepository,
							IShoppingCartRepository shoppingCartRepository,
							UserManager<ApplicationUser> userManager,
							IRepository<OrderItem, int> syncOrderItemRepository,
							IAsyncRepository<OrderItem, int> asyncOrderItemRepository,
							ICheckoutRepository checkoutRepository,
							IProductRepository productRepository)
		{
			this._orderRepository = orderRepository;
			this._shoppingCartRepository = shoppingCartRepository;
			this._userManager = userManager;
			this._syncOrderItemRepository = syncOrderItemRepository;
			this._asyncOrderItemRepository = asyncOrderItemRepository;
			this._checkoutRepository = checkoutRepository;
			this._productRepository = productRepository;
		}

		public async Task<int?> CreateOrderAsync(Checkout? checkout)
		{
			int? newOrderId = null;

			if (checkout != null)
			{
				if (checkout.UserId != null)
				{
					Order order = new Order
					{
						UserId = checkout.UserId,
						TotalAmount = checkout.TotalPrice,
						Status = DefaultStartingOrderStatus,
						ShippingAddressId = checkout.ShippingAddressId!.Value,
						BillingAddressId = checkout.BillingAddressId.HasValue ?
												checkout.BillingAddressId.Value : 
													checkout.ShippingAddressId.Value,
						PaymentMethodId = checkout.PaymentMethodId,
						PaymentDetails = checkout.PaymentDetails,
						ShippingOption = checkout.ShippingOption,
						EstimatedDeliveryStart = checkout.EstimatedDeliveryStart,
						EstimatedDeliveryEnd = checkout.EstimatedDeliveryEnd,
						ShippingPrice = checkout.ShippingPrice,
						CheckoutId = checkout.Id,
						Checkout = checkout,
						OrderNumber = GenerateUniqueOrderNumber(),
					};

					OrderItem[] orderItems = checkout.ShoppingCart.ShoppingCartItems
						.Select(item => new OrderItem
						{
							ProductId = item.ProductId,
							OrderId = order.Id,
							Quantity = item.Quantity,
							UnitPrice = item.Price,
							ProductSize = item.ProductSize,
							Subtotal = item.TotalPrice,

						}).ToArray();

					order.OrderItems = orderItems;

					await this._orderRepository.AddAsync(order);

					newOrderId = order.Id;
					checkout.Order = order;
				}
				else
				{
					Order order = new Order
					{
						GuestId = checkout.GuestId,
						GuestName = checkout.GuestName,
						GuestEmail = checkout.GuestEmail,
						TotalAmount = checkout.TotalPrice,
						Status = DefaultStartingOrderStatus,
						ShippingAddressId = checkout.ShippingAddressId!.Value,
						BillingAddressId = checkout.BillingAddressId.HasValue ?
												checkout.BillingAddressId.Value :
													checkout.ShippingAddressId.Value,
						PaymentMethodId = checkout.PaymentMethodId,
						PaymentDetails = checkout.PaymentDetails,
						ShippingOption = checkout.ShippingOption,
						EstimatedDeliveryStart = checkout.EstimatedDeliveryStart,
						EstimatedDeliveryEnd = checkout.EstimatedDeliveryEnd,
						ShippingPrice = checkout.ShippingPrice,
						CheckoutId = checkout.Id,
						Checkout = checkout,
						OrderNumber = GenerateUniqueOrderNumber(),
					};

					OrderItem[] orderItems = checkout.ShoppingCart.ShoppingCartItems
						.Select(item => new OrderItem
						{
							ProductId = item.ProductId,
							OrderId = order.Id,
							Quantity = item.Quantity,
							UnitPrice = item.Price,
							Subtotal = item.TotalPrice,
							ProductSize = item.ProductSize,
						}).ToArray();

					order.OrderItems = orderItems;

					await this._orderRepository.AddAsync(order);

					newOrderId = order.Id;
					checkout.Order = order;
				}

				if (newOrderId != null)
				{
					await this._shoppingCartRepository.ClearShoppingCartItemsAsync(checkout.ShoppingCart.Id);

					checkout.CompletedAt = DateTime.UtcNow;
					checkout.IsCompleted = true;
					await this._checkoutRepository.UpdateAsync(checkout);
				}

			}

			return newOrderId;
		}

		public async Task<IEnumerable<UserOrderViewModel>?> GetUserOrdersAsync(string? userId)
		{
			IEnumerable<UserOrderViewModel>? model = null;

			if (userId != null)
			{
				ApplicationUser? user = await this._userManager
										.FindByIdAsync(userId);

				if (user != null)
				{
					var orders = await this._orderRepository
							.GetAllAttached()
							.AsNoTracking()
							.Where(o => o.UserId == user.Id)
							.ToListAsync();

					model = orders.Select(order => new UserOrderViewModel
					{
						OrderNumber = order.OrderNumber,
						OrderDate = order.OrderDate,
						EstimatedDeliveryStart = order.EstimatedDeliveryStart,
						EstimatedDeliveryEnd = order.EstimatedDeliveryEnd,
						TotalAmount = order.TotalAmount,
						ShippingOption = order.ShippingOption,
						Status = order.Status.ToString(),
						IsCompleted = order.IsCompleted,
						IsCancelled = order.IsCancelled,
						Items = order.OrderItems.Select(oi => new UserOrderItemViewModel
						{
							Name = oi.Product.Name,
							Quantity = oi.Quantity,
							Price = oi.UnitPrice,
							ImageUrl = oi.Product.ImageUrl
						}).ToList()
					})
					.OrderByDescending(o => o.OrderDate)
					.ToList();
				}
			}

			return model;
		}

		public async Task<bool> GetGuestOrderAsync(string? userId, TrackGuestOrderViewModel? model)
		{
			bool result = false;

			if (model != null)
			{
				model.HasSearched = true;

				bool isFormValid = !string.IsNullOrWhiteSpace(userId) &&
								   !string.IsNullOrWhiteSpace(model.GuestEmail) &&
								   !string.IsNullOrWhiteSpace(model.OrderNumber);

				if (isFormValid)
				{
					Order? order = await this._orderRepository
								.GetAllAttached()
								.AsNoTracking()
								.Where(o => o.GuestEmail!.ToLower() == model.GuestEmail!.ToLower() &&
											o.OrderNumber.ToLower() == model.OrderNumber!.ToLower())
								.SingleOrDefaultAsync();

					bool isOrderValid = order != null && order.GuestId == userId;

					if (isOrderValid)
					{
						model.OrderFound = true;
						model.Status = order!.Status.ToString();
						model.TotalAmount = order.TotalAmount;
						model.ShippingOption = order.ShippingOption;
						model.EstimatedDeliveryStartFormatted = order.EstimatedDeliveryStart.ToString("MMM dd");
						model.EstimatedDeliveryEndFormatted = order.EstimatedDeliveryEnd.ToString("MMM dd");
						model.Items = order.OrderItems.Select(i => new OrderProductViewModel
						{
							Name = i.Product.Name,
							Quantity = i.Quantity,
							Price = i.UnitPrice,
							ProductSize = i.ProductSize,
							ImageUrl = i.Product.ImageUrl
						}).ToList();

						result = true;
					}
				}
			}

			return result;
		}

		public async Task<OrderDetailsViewModel?> GetOrderDetailsAsync(string? orderNumber)
		{
			OrderDetailsViewModel? details = null;

			if (!string.IsNullOrWhiteSpace(orderNumber))
			{
				Order? order = await this._orderRepository
								.SingleOrDefaultAsync(o => o.OrderNumber.ToLower() == orderNumber.ToLower());

				if (order != null)
				{
					bool isBillingSameAsShipping = IsBillingSameAsShipping(order.ShippingAddress, order.BillingAddress);

					details = new OrderDetailsViewModel()
					{
						OrderNumber = order.OrderNumber,
						OrderDate = order.OrderDate,
						Status = order.Status.ToString(),
						ShippingOption = order.ShippingOption,
						EstimatedDeliveryStart = order.EstimatedDeliveryStart,
						EstimatedDeliveryEnd = order.EstimatedDeliveryEnd,
						TotalAmount = order.TotalAmount,
						ShippingPrice = order.ShippingPrice,
						PaymentMethodName = order.PaymentMethod.Name,
						PaymentMethodCode = order.PaymentMethod.Code!.Value,
						PaymentDetails = order.PaymentDetails != null ? new OrderPaymentDetailsViewModel()
						{
							NameOnCard = order.PaymentDetails.NameOnCard,
							CardNumberMasked = MaskCardNumber(order.PaymentDetails.CardNumber),
							ExpMonth = order.PaymentDetails.ExpMonth,
							ExpYear = order.PaymentDetails.ExpYear,
							PaymentStatus = order.PaymentDetails.Status.ToString()
						} : null,
						ShippingAddress = new AddressViewModel()
						{
							Street = order.ShippingAddress.Street,
							City = order.ShippingAddress.City,
							Country = order.ShippingAddress.Country,
							ZipCode = order.ShippingAddress.ZipCode,
							PhoneNumber = order.ShippingAddress.PhoneNumber
						},
						BillingAddress = isBillingSameAsShipping ? null : new AddressViewModel()
						{
							Street = order.BillingAddress.Street,
							City = order.BillingAddress.City,
							Country = order.BillingAddress.Country,
							ZipCode = order.BillingAddress.ZipCode,
							PhoneNumber = order.BillingAddress.PhoneNumber
						},
						Items = order.OrderItems.Select(i => new OrderProductViewModel()
						{
							Name = i.Product.Name,
							Price = i.UnitPrice,
							Quantity = i.Quantity,
							ProductSize = i.ProductSize,
							ImageUrl = i.Product.ImageUrl
						}).ToList()
					};
				}
			}

			return details;
		}

		public async Task<OrderConfirmationViewModel?> GetOrderForConfirmationPageAsync(string? userId, int? orderId)
		{
			OrderConfirmationViewModel? model = null;

			if (userId != null && orderId != null)
			{
				Order? order = await this._orderRepository
								.GetByIdAsync(orderId.Value);

				if (order != null)
				{
					OrderItem[] orderItems = await this._syncOrderItemRepository
													.GetAllAttached()
													.AsNoTracking()
													.Include(oi => oi.Product)
													.Where(oi => oi.OrderId == orderId)
													.ToArrayAsync();

					//In case the Product is not loaded in the OrderItems!
					foreach (var orderItem in orderItems)
					{
						if (orderItem.Product == null)
						{
							int productId = orderItem.ProductId;
							Product? product = await this._productRepository
											.GetByIdAsync(productId);

							if (product != null)
							{
								orderItem.Product = product;
								await this._asyncOrderItemRepository.UpdateAsync(orderItem);
							}
						}
					}

					List<OrderProductViewModel> orderProducts = orderItems
									.Select(oi => new OrderProductViewModel()
									{
										Name = oi.Product.Name,
										Quantity = oi.Quantity,
										Price = oi.Subtotal,
										ProductSize = oi.ProductSize,
										ImageUrl = oi.Product.ImageUrl
									})
									.ToList();

					ApplicationUser? user = await this._userManager
								.FindByIdAsync(userId);

					if (user != null)
					{
						bool isOrderToUser = user.Orders
									.Any(o => o.Id == order.Id) && 
										   order.UserId != null &&
											  order.UserId == userId;

						if (isOrderToUser)
						{
							model = new OrderConfirmationViewModel
							{
								OrderNumber = order.OrderNumber,
								EstimatedDeliveryStart = order.EstimatedDeliveryStart,
								EstimatedDeliveryEnd = order.EstimatedDeliveryEnd,
								ShippingOption = order.ShippingOption,
								ShippingPrice = order.ShippingPrice,
								TotalAmount = order.TotalAmount,
								RecipientName = order.User!.UserName!,
								UserEmail = order.User!.Email!,
								ShippingAddress = FormatAddress(order.ShippingAddress),
								IsGuest = false,
								Products = orderProducts
							};
						}
					}
					else
					{
						bool isOrderToUser = order.GuestId != null && 
											 order.GuestId == userId;

						if (isOrderToUser)
						{
							model = new OrderConfirmationViewModel
							{
								OrderNumber = order.OrderNumber,
								EstimatedDeliveryStart = order.EstimatedDeliveryStart,
								EstimatedDeliveryEnd = order.EstimatedDeliveryEnd,
								ShippingOption = order.ShippingOption,
								ShippingPrice = order.ShippingPrice,
								TotalAmount = order.TotalAmount,
								RecipientName = order.GuestName!,
								UserEmail = order.GuestEmail!,
								ShippingAddress = FormatAddress(order.ShippingAddress),
								IsGuest = true,
								Products = orderProducts
							};
						}
					}
				}
				else
				{
					throw new InvalidOperationException("The order is not found!");
				}
			}

			return model;
		}


		private static string GenerateUniqueOrderNumber()
		{
			const string prefix = "ORD";
			string datePart = DateTime.UtcNow.ToString("yyyyMMdd");

			// Generate 5 random bytes (10 hex chars)
			byte[] randomBytes = new byte[5];
			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(randomBytes);
			}

			StringBuilder randomPart = new StringBuilder(10);
			foreach (byte b in randomBytes)
			{
				randomPart.Append(b.ToString("X2")); // uppercase hex
			}

			return $"{prefix}-{datePart}-{randomPart}";
		}

		private static string FormatAddress(Address address)
		{
			StringBuilder str = new StringBuilder();

			string streetAddress = address.Street.ToString();
			string city = address.City.ToString();
			string zipCode = address.ZipCode.ToString();
			string country = address.Country.ToString();
			string phoneNumber = address.PhoneNumber.ToString();

			str.AppendLine(streetAddress);

			str.AppendLine($"{city}, {zipCode}");

			str.AppendLine(country);

			str.Append($"Phone: {phoneNumber}");

			return str.ToString().TrimEnd();
		}

		private static string MaskCardNumber(string cardNumber)
		{
			string maskedCardNumber = cardNumber.Length > 4
							? new string('*', cardNumber.Length - 4) + cardNumber.Substring(cardNumber.Length - 4)
							: cardNumber;

			return maskedCardNumber;
		}

		private static bool IsBillingSameAsShipping(Address shipping, Address billing)
		{
			bool areSame = true;

			if (!shipping.Street.Equals(billing.Street))
				areSame = false;

			if (!shipping.City.Equals(billing.City))
				areSame = false;

			if (!shipping.Country.Equals(billing.Country))
				areSame = false;

			if (!shipping.ZipCode.Equals(billing.ZipCode))
				areSame = false;

			if (!shipping.PhoneNumber.Equals(billing.PhoneNumber))
				areSame = false;

			return areSame;
		}
	}
}

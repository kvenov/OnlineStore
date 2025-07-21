using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models;
using OnlineStore.Data.Models.Enums;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.Checkout;
using OnlineStore.Web.ViewModels.Checkout.Partials;

using static OnlineStore.Common.ApplicationConstants;

namespace OnlineStore.Services.Core
{
	public class CheckoutService : ICheckoutService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IShoppingCartRepository _shoppingCartRepository;
		private readonly ICheckoutRepository _checkoutRepository;
		private readonly IOrderRepository _orderRepository;
		private readonly IAsyncRepository<PaymentMethod, int> _paymentMethodRepository;

		public CheckoutService(UserManager<ApplicationUser> userManager, 
							   IShoppingCartRepository shoppingCartRepository, 
							   ICheckoutRepository checkoutRepository, 
							   IOrderRepository orderRepository,
							   IAsyncRepository<PaymentMethod, int> paymentMethodRepository)
		{
			this._userManager = userManager;
			this._shoppingCartRepository = shoppingCartRepository;
			this._checkoutRepository = checkoutRepository;
			this._orderRepository = orderRepository;
			this._paymentMethodRepository = paymentMethodRepository;
		}


		public async Task<Checkout?> InitializeCheckoutAsync(string? userId)
		{
			Checkout? checkout = null;
			if (userId != null)
			{
				ApplicationUser? user = await _userManager
							.FindByIdAsync(userId);

				if (user != null)
				{
					var existingCheckout = await this._checkoutRepository
							.SingleOrDefaultAsync(c =>
								c.UserId == user.Id && c.Order == null);

					if (existingCheckout != null)
					{
						checkout = existingCheckout;
						return checkout;
					}

					var shoppingCart = await this._shoppingCartRepository
							.SingleOrDefaultAsync(sc => sc.UserId == user.Id);

					decimal totalPrice = await this._shoppingCartRepository
									.GetItemsTotalPrice(userId);

					var newCheckout = new Checkout
					{
						UserId = user.Id,
						ShoppingCartId = shoppingCart!.Id,
						StartedAt = DateTime.UtcNow,
						TotalPrice = totalPrice
					};

					var lastOrder = await this._orderRepository
						.GetAllAttached()
						.Where(o => o.UserId == user.Id)
						.OrderByDescending(o => o.OrderDate)
						.FirstOrDefaultAsync();

					if (lastOrder != null)
					{
						newCheckout.ShippingAddressId = lastOrder.ShippingAddressId;
						newCheckout.BillingAddressId = lastOrder.BillingAddressId;
						newCheckout.PaymentMethodId = lastOrder.PaymentMethodId;

						if (lastOrder.PaymentDetails != null)
						{
							newCheckout.PaymentDetailsId = lastOrder.PaymentDetails.Id;
						}
					}
					else
					{
						var defaultPaymentMethod = await this._paymentMethodRepository
							.FirstOrDefaultAsync(pm => pm.Name == DefaultPaymentMethodName);

						if (defaultPaymentMethod != null)
						{
							newCheckout.PaymentMethodId = defaultPaymentMethod.Id;
						}
						else
						{
							throw new InvalidOperationException("No available payment methods configured.");
						}
					}

					await this._checkoutRepository.AddAsync(newCheckout);
					await this._checkoutRepository.SaveChangesAsync();

					checkout = newCheckout;
				}
				else
				{
					var existingCheckout = await this._checkoutRepository
							.SingleOrDefaultAsync(c =>
								c.GuestId == userId && c.Order == null);

					if (existingCheckout != null)
					{
						checkout = existingCheckout;
						return checkout;
					}

					var shoppingCart = await this._shoppingCartRepository
							.SingleOrDefaultAsync(sc => sc.GuestId == userId);

					decimal totalPrice = await this._shoppingCartRepository
									.GetItemsTotalPrice(userId);

					var newCheckout = new Checkout
					{
						GuestId = userId,
						ShoppingCartId = shoppingCart!.Id,
						StartedAt = DateTime.UtcNow,
						TotalPrice = totalPrice
					};

					var lastOrder = await this._orderRepository
						.GetAllAttached()
						.Where(o => o.GuestId == userId)
						.OrderByDescending(o => o.OrderDate)
						.FirstOrDefaultAsync();

					if (lastOrder != null)
					{
						newCheckout.ShippingAddressId = lastOrder.ShippingAddressId;
						newCheckout.BillingAddressId = lastOrder.BillingAddressId;
						newCheckout.PaymentMethodId = lastOrder.PaymentMethodId;

						if (lastOrder.PaymentDetails != null)
						{
							newCheckout.PaymentDetailsId = lastOrder.PaymentDetails.Id;
						}
					}
					else
					{
						var defaultPaymentMethod = await this._paymentMethodRepository
							.FirstOrDefaultAsync(pm => pm.Name == DefaultPaymentMethodName);

						if (defaultPaymentMethod != null)
						{
							newCheckout.PaymentMethodId = defaultPaymentMethod.Id;
						}
						else
						{
							throw new InvalidOperationException("No available payment methods configured.");
						}
					}

					await this._checkoutRepository.AddAsync(newCheckout);
					await this._checkoutRepository.SaveChangesAsync();

					checkout = newCheckout;
				}
			}

			return checkout;
		}

		public CheckoutViewModel? GetUserCheckout(Checkout? checkout)
		{
			CheckoutViewModel? model = null;

			if (checkout != null)
			{
				ApplicationUser? user = checkout.User;

				if (!string.IsNullOrWhiteSpace(checkout.UserId) && (user != null))
				{
					MemberAddressViewModel addressViewModel = new MemberAddressViewModel();
					if (user.Addresses.Any())
					{
						addressViewModel.SavedAddresses = user.Addresses
							.Select(a => new MemberAddressItemViewModel
							{
								Id = a.Id,
								Street = a.Street,
								City = a.City,
								ZipCode = a.ZipCode,
								Country = a.Country,
								PhoneNumber = a.PhoneNumber,
								IsShippingAddress = a.IsShippingAddress
							}).ToList();
					}

					if ((checkout.ShippingAddressId != null) 
							&& (checkout.BillingAddressId != null))
					{
						addressViewModel.SelectedShippingAddressId = checkout.ShippingAddressId.Value;
						addressViewModel.SelectedBillingAddressId = checkout.BillingAddressId.Value;
					}

					ShippingOptionsViewModel shippingModel = CreateShippingOptions(user);

					
					string paymentMethodName = checkout.PaymentMethod.Name.Replace(" ", "");
					PaymentMethodCode code = Enum.TryParse<PaymentMethodCode>(paymentMethodName, out var parsedCode)
						? parsedCode
						: PaymentMethodCode.CreditCard;

					PaymentMethodViewModel paymentModel = new PaymentMethodViewModel{
						SelectedPaymentOption = checkout.PaymentMethod.Code != null
								? checkout.PaymentMethod.Code.Value
								: code
					};

					if (checkout.PaymentDetailsId != null && checkout.PaymentDetails != null)
					{
						PaymentDetails lastPaymentDetails = checkout.PaymentDetails;

						paymentModel.CreditCardDetails = new CreditCardFormViewModel()
						{
							CardNumber = lastPaymentDetails.CardNumber,
							NameOnCard = lastPaymentDetails.NameOnCard,
							ExpMonth = lastPaymentDetails.ExpMonth!.Value,
							ExpYear = lastPaymentDetails.ExpYear!.Value,
						};
					}

					OrderSummaryViewModel orderSummary = new OrderSummaryViewModel
					{
						Products = checkout.ShoppingCart
											.ShoppingCartItems
											.Select(item => new OrderProductViewModel()
											{
												ProductId = item.ProductId,
												Name = item.Product.Name,
												UnitPrice = item.Price,
												Quantity = item.Quantity,
												ImageUrl = item.Product.ImageUrl,
												Size = item.ProductSize
											})
											.ToList(),
						DeliveryCost = 0,
						Subtotal = checkout.TotalPrice
					};

					model = new CheckoutViewModel
					{
						IsGuest = false,
						UserId = checkout.UserId,
						MemberAddress = addressViewModel,
						Shipping = shippingModel,
						Payment = paymentModel,
						Summary = orderSummary
					};
				}
				else
				{
					GuestAddressViewModel addressViewModel = new GuestAddressViewModel();

					if ((checkout.ShippingAddressId != null) && (checkout.BillingAddressId != null) &&
						(checkout.ShippingAddress != null) && (checkout.BillingAddress != null))
					{
						addressViewModel.ShippingAddress = new GuestShippingAddressViewModel
						{
							Street = checkout.ShippingAddress.Street,
							City = checkout.ShippingAddress.City,
							ZipCode = checkout.ShippingAddress.ZipCode,
							Country = checkout.ShippingAddress.Country,
							PhoneNumber = checkout.ShippingAddress.PhoneNumber
						};

						addressViewModel.BillingAddress = new GuestBillingAddressViewModel
						{
							BillingStreet = checkout.BillingAddress.Street,
							BillingCity = checkout.BillingAddress.City,
							BillingZipCode = checkout.BillingAddress.ZipCode,
							BillingCountry = checkout.BillingAddress.Country,
							BillingPhoneNumber = checkout.BillingAddress.PhoneNumber
						};
					}

					ShippingOptionsViewModel shippingModel = CreateShippingOptions(user);

					string paymentMethodName = checkout.PaymentMethod.Name.Replace(" ", "");
					PaymentMethodCode code = Enum.TryParse<PaymentMethodCode>(paymentMethodName, out var parsedCode)
						? parsedCode
						: PaymentMethodCode.CreditCard;

					PaymentMethodViewModel paymentModel = new PaymentMethodViewModel
					{
						SelectedPaymentOption = checkout.PaymentMethod.Code != null
								? checkout.PaymentMethod.Code.Value
								: code
					};

					decimal deliveryCost = checkout.TotalPrice >= MinPriceForFreeShipping ?
									StandartShippingPriceForMembers :
											StandartShippingPriceForGuests;

					OrderSummaryViewModel orderSummary = new OrderSummaryViewModel
					{
						Products = checkout.ShoppingCart
											.ShoppingCartItems
											.Select(item => new OrderProductViewModel()
											{
												ProductId = item.ProductId,
												Name = item.Product.Name,
												UnitPrice = item.Price,
												Quantity = item.Quantity,
												ImageUrl = item.Product.ImageUrl,
												Size = item.ProductSize
											})
											.ToList(),
						DeliveryCost = deliveryCost,
						Subtotal = checkout.TotalPrice
					};

					model = new CheckoutViewModel
					{
						IsGuest = true,
						GuestId = checkout.GuestId,
						GuestAddress = addressViewModel,
						Shipping = shippingModel,
						Payment = paymentModel,
						Summary = orderSummary
					};
				}
			}

			return model;
		}


		private static ShippingOptionsViewModel CreateShippingOptions(ApplicationUser? user)
		{
			var today = DateTime.Today;

			var standardStart = AddBusinessDays(today, 5);
			var standardEnd = AddBusinessDays(today, 7);

			var expressStart = AddBusinessDays(today, 2);
			var expressEnd = AddBusinessDays(today, 3);

			var isMember = user != null;

			var options = new List<ShippingOptionItemViewModel>
			{
				new ShippingOptionItemViewModel
				{
					Id = 1,
					Name = "Standard Delivery",
					Description = isMember ? "Free shipping for members" : "Delivered via standard carrier",
					DateRange = $"{standardStart:MMM dd} – {standardEnd:MMM dd}",
					Price = isMember ? StandartShippingPriceForMembers : StandartShippingPriceForGuests
				},
				new ShippingOptionItemViewModel
				{
					Id = 2,
					Name = "Express Delivery",
					Description = "Fastest option via premium courier",
					DateRange = $"{expressStart:MMM dd} – {expressEnd:MMM dd}",
					Price = ExpressShippingPrice
				}
			};

			return new ShippingOptionsViewModel
			{
				SelectedShippingOptionId = options.First().Id,
				Options = options
			};
		}

		private static DateTime AddBusinessDays(DateTime startDate, int businessDays)
		{
			var current = startDate;
			while (businessDays > 0)
			{
				current = current.AddDays(1);
				if (current.DayOfWeek != DayOfWeek.Saturday && current.DayOfWeek != DayOfWeek.Sunday)
				{
					businessDays--;
				}
			}
			return current;
		}
	}
}

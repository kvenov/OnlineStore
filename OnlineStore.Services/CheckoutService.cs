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
								c.UserId == user.Id);

					if (existingCheckout != null)
					{
						await this._checkoutRepository.RefreshCheckoutTotalsAsync(userId, existingCheckout.Id);
						return existingCheckout;
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

					//Here we set the checkout to use user defaults, if they exist.
					SetCheckoutDefaultsFromUser(newCheckout, user);

					if (IsCheckoutMissingEssentialData(newCheckout))
					{
						var lastOrder = await this._orderRepository
						.GetAllAttached()
						.Where(o => o.UserId == user.Id)
						.OrderByDescending(o => o.OrderDate)
						.FirstOrDefaultAsync();

						if (lastOrder != null)
						{
							SetCheckoutDefaultsFromOrder(newCheckout, lastOrder);
						}
					}

					await this.SetCheckoutDefaultPaymentMethod(newCheckout);

					await this._checkoutRepository.AddAsync(newCheckout);
					await this._checkoutRepository.SaveChangesAsync();

					checkout = newCheckout;
				}
				else
				{
					var existingCheckout = await this._checkoutRepository
							.SingleOrDefaultAsync(c =>
								c.GuestId == userId);

					if (existingCheckout != null)
					{
						await this._checkoutRepository.RefreshCheckoutTotalsAsync(userId, existingCheckout.Id);
						return existingCheckout;
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

					if (this.IsCheckoutMissingEssentialData(newCheckout))
					{
						var lastOrder = await this._orderRepository
						.GetAllAttached()
						.Where(o => o.GuestId == userId)
						.OrderByDescending(o => o.OrderDate)
						.FirstOrDefaultAsync();

						if (lastOrder != null)
						{
							this.SetCheckoutDefaultsFromOrder(newCheckout, lastOrder);
						}
					}

					await this.SetCheckoutDefaultPaymentMethod(newCheckout);

					await this._checkoutRepository.AddAsync(newCheckout);
					await this._checkoutRepository.SaveChangesAsync();

					checkout = newCheckout;
				}
			}

			return checkout;
		}

		public async Task<CheckoutViewModel?> GetUserCheckout(Checkout? checkout)
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

					if (checkout.ShippingAddressId.HasValue)
					{
						addressViewModel.SelectedShippingAddressId = checkout.ShippingAddressId;
					}

					if (checkout.BillingAddressId.HasValue)
					{
						addressViewModel.SelectedBillingAddressId = checkout.BillingAddressId;
					}

					ShippingOptionsViewModel shippingModel = CreateShippingOptions(user);

					PaymentMethodViewModel paymentModel = new PaymentMethodViewModel{
						SelectedPaymentOption = checkout.PaymentMethod.Code!.Value
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


					decimal deliveryCost = await this._checkoutRepository
							.GetCheckoutDeliveryCostAsync(checkout.UserId, checkout.TotalPrice);

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

					if ((checkout.ShippingAddressId != null) &&
						(checkout.ShippingAddress != null))
					{
						addressViewModel.ShippingAddress = new GuestShippingAddressViewModel
						{
							Street = checkout.ShippingAddress.Street,
							City = checkout.ShippingAddress.City,
							ZipCode = checkout.ShippingAddress.ZipCode,
							Country = checkout.ShippingAddress.Country,
							PhoneNumber = checkout.ShippingAddress.PhoneNumber
						};
					}

					if ((checkout.BillingAddressId != null) && 
						(checkout.BillingAddress != null))
					{
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

					PaymentMethodViewModel paymentModel = new PaymentMethodViewModel
					{
						SelectedPaymentOption = checkout.PaymentMethod.Code!.Value
					};

					decimal deliveryCost = await this._checkoutRepository
								.GetCheckoutDeliveryCostAsync(checkout.GuestId, checkout.TotalPrice);

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


		private void SetCheckoutDefaultsFromUser(Checkout checkout, ApplicationUser user)
		{
			if (user.DefaultShippingAddressId.HasValue)
			{
				checkout.ShippingAddressId = user.DefaultShippingAddressId;
				checkout.ShippingAddress = user.DefaultShippingAddress;
			}

			if (user.DefaultBillingAddressId.HasValue)
			{
				checkout.BillingAddressId = user.DefaultBillingAddressId;
				checkout.BillingAddress = user.DefaultBillingAddress;
			}

			if ((user.DefaultPaymentMethodId.HasValue) && (user.DefaultPaymentMethod != null))
			{
				checkout.PaymentMethodId = user.DefaultPaymentMethodId.Value;
				checkout.PaymentMethod = user.DefaultPaymentMethod;
			}

			if (user.DefaultPaymentDetailsId.HasValue)
			{
				checkout.PaymentDetailsId = user.DefaultPaymentDetailsId;
				checkout.PaymentDetails = user.DefaultPaymentDetails;
			}
		}

		private void SetCheckoutDefaultsFromOrder(Checkout checkout, Order lastOrder)
		{
			if (checkout.ShippingAddressId == null)
			{
				checkout.ShippingAddressId = lastOrder.ShippingAddressId;
				checkout.ShippingAddress = lastOrder.ShippingAddress;
			}

			if (checkout.BillingAddressId == null)
			{
				checkout.BillingAddressId = lastOrder.BillingAddressId;
				checkout.BillingAddress = lastOrder.BillingAddress;
			}

			if (checkout.PaymentMethodId == null || int.IsNegative(checkout.PaymentMethodId))
			{
				checkout.PaymentMethodId = lastOrder.PaymentMethodId;
				checkout.PaymentMethod = lastOrder.PaymentMethod;
			}

			if (checkout.PaymentDetailsId == null && lastOrder.PaymentDetails != null)
			{
				checkout.PaymentDetailsId = lastOrder.PaymentDetails.Id;
				checkout.PaymentDetails = lastOrder.PaymentDetails;
			}
		}

		private async Task SetCheckoutDefaultPaymentMethod(Checkout checkout)
		{
			if (checkout.PaymentMethodId == null || int.IsNegative(checkout.PaymentMethodId))
			{
				PaymentMethodCode defaultPaymentMethodCode = Enum.TryParse(DefaultPaymentMethodCode, out PaymentMethodCode code)
									? code : PaymentMethodCode.CreditCard;

				PaymentMethod? defaultPaymentMethod = await _paymentMethodRepository
						.FirstOrDefaultAsync(pm => pm.Name == DefaultPaymentMethodName &&
												   pm.Code == defaultPaymentMethodCode);

				if (defaultPaymentMethod == null)
					throw new InvalidOperationException("The payment method cannot be found!.");

				checkout.PaymentMethodId = defaultPaymentMethod.Id;
				checkout.PaymentMethod = defaultPaymentMethod;
			}
		}

		private bool IsCheckoutMissingEssentialData(Checkout checkout)
		{
			return checkout.ShippingAddressId == null
				|| checkout.BillingAddressId == null
				|| checkout.PaymentMethodId == null;
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
					Name = "Standard Delivery",
					Description = isMember ? "Free shipping for members" : "Delivered via standard carrier",
					DateRange = $"{standardStart:MMM dd} – {standardEnd:MMM dd}",
					Price = isMember ? StandartShippingPriceForMembers : StandartShippingPriceForGuests
				},
				new ShippingOptionItemViewModel
				{
					Name = "Express Delivery",
					Description = "Fastest option via premium courier",
					DateRange = $"{expressStart:MMM dd} – {expressEnd:MMM dd}",
					Price = ExpressShippingPrice
				}
			};

			return new ShippingOptionsViewModel
			{
				SelectedShippingOption = options.First(),
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

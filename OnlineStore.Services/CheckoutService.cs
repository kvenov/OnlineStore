using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models;
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

		public CheckoutService(UserManager<ApplicationUser> userManager, IShoppingCartRepository shoppingCartRepository)
		{
			this._userManager = userManager;
			this._shoppingCartRepository = shoppingCartRepository;
		}

		public async Task<CheckoutViewModel?> GetUserCheckoutAsync(string? userId)
		{
			CheckoutViewModel? model = null;

			if (userId != null)
			{
				ApplicationUser? user = await _userManager
							.FindByIdAsync(userId);

				if (user != null)
				{
					MemberAddressViewModel addressViewModel = new MemberAddressViewModel
					{
						SavedAddresses = user.Addresses
							.Select(a => new MemberAddressItemViewModel
							{
								Id = a.Id,
								Street = a.Street,
								City = a.City,
								ZipCode = a.ZipCode,
								Country = a.Country,
								PhoneNumber = a.PhoneNumber
							}).ToList()
					};

					ShippingOptionsViewModel shippingModel = CreateShippingOptions(user);

					PaymentMethodViewModel paymentModel = new PaymentMethodViewModel();

					if (user.Orders.Any())
					{
						Order lastOrder = user.Orders
							.OrderByDescending(o => o.OrderDate)
							.First();

						PaymentDetails? lastPaymentDetails = lastOrder.PaymentDetails;

						if (lastPaymentDetails != null)
						{
							paymentModel.CreditCardDetails = new CreditCardFormViewModel()
							{
								CardNumber = lastPaymentDetails.CardNumber,
								NameOnCard = lastPaymentDetails.NameOnCard,
								ExpMonth = lastPaymentDetails.ExpMonth!.Value,
								ExpYear = lastPaymentDetails.ExpYear!.Value,
							};
						}
					}

					OrderSummaryViewModel orderSummary = new OrderSummaryViewModel
					{
						Products = await _shoppingCartRepository
											.GetAllAttached()
											.Include(sc => sc.ShoppingCartItems)
											.ThenInclude(sci => sci.Product)
											.Where(sc => sc.UserId == userId)
											.SelectMany(sc => sc.ShoppingCartItems)
											.Select(item => new OrderProductViewModel()
											{
												ProductId = item.ProductId,
												Name = item.Product.Name,
												UnitPrice = item.Price,
												Quantity = item.Quantity,
												ImageUrl = item.Product.ImageUrl,
												Size = item.ProductSize
											})
											.ToListAsync(),
						DeliveryCost = 0,
						Subtotal = await _shoppingCartRepository
											.GetItemsTotalPrice(userId),
					};

					model = new CheckoutViewModel
					{
						IsGuest = false,
						UserId = userId,
						MemberAddress = addressViewModel,
						Shipping = shippingModel,
						Payment = paymentModel,
						Summary = orderSummary
					};
				}
				else
				{
					GuestAddressViewModel addressViewModel = new GuestAddressViewModel();
					ShippingOptionsViewModel shippingModel = CreateShippingOptions(user);

					PaymentMethodViewModel paymentModel = new PaymentMethodViewModel();

					OrderSummaryViewModel orderSummary = new OrderSummaryViewModel
					{
						Products = await _shoppingCartRepository
											.GetAllAttached()
											.Include(sc => sc.ShoppingCartItems)
											.ThenInclude(sci => sci.Product)
											.Where(sc => sc.GuestId == userId)
											.SelectMany(sc => sc.ShoppingCartItems)
											.Select(item => new OrderProductViewModel()
											{
												ProductId = item.ProductId,
												Name = item.Product.Name,
												UnitPrice = item.Price,
												Quantity = item.Quantity,
												ImageUrl = item.Product.ImageUrl,
												Size = item.ProductSize
											})
											.ToListAsync(),
						Subtotal = await _shoppingCartRepository
											.GetItemsTotalPrice(userId),
					};

					model = new CheckoutViewModel
					{
						IsGuest = true,
						GuestId = userId,
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

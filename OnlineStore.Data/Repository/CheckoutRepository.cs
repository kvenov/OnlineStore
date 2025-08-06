using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models;
using OnlineStore.Data.Models.Enums;
using OnlineStore.Data.Repository.Interfaces;

using static OnlineStore.Common.ApplicationConstants;

namespace OnlineStore.Data.Repository
{
	public class CheckoutRepository : BaseRepository<Checkout, int>, ICheckoutRepository
	{
		private readonly IOrderRepository _orderRepository;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IAsyncRepository<PaymentMethod, int> _paymentMethodRepository;

		public CheckoutRepository(ApplicationDbContext dbContext, 
								  IOrderRepository orderRepository, 
								  UserManager<ApplicationUser> userManager,
								  IAsyncRepository<PaymentMethod, int> paymentMethodRepository) 
					: base(dbContext)
		{
			this._orderRepository = orderRepository;
			this._userManager = userManager;
			this._paymentMethodRepository = paymentMethodRepository;
		}

		public async Task<decimal> GetCheckoutDeliveryCostAsync(string? userId, decimal? subTotal)
		{
			if (userId == null || subTotal == null)
			{
				throw new InvalidCastException("User ID or Checkout ID cannot be null.");
			}

			ApplicationUser? user = await this.DbContext.Users
				.SingleOrDefaultAsync(u => u.Id == userId);

			decimal deliveryCost = StandartShippingPriceForMembers;
			if (user == null)
			{
				deliveryCost = subTotal >= MinPriceForFreeShipping ?
												StandartShippingPriceForMembers :
													StandartShippingPriceForGuests;
			}

			return deliveryCost;
		}

		public async Task RefreshCheckoutStartingDateAsync(int checkoutId)
		{
			Checkout? checkout = await this
						.SingleOrDefaultAsync(c => c.Id == checkoutId);

			if (checkout == null)
				throw new InvalidOperationException("Checkout not found.");

			checkout.StartedAt = DateTime.UtcNow;
			await this.UpdateAsync(checkout);
		}

		public async Task RefreshCheckoutTotalsAsync(string? userId, int checkoutId)
		{
			ShoppingCart? cart = await this.DbContext.ShoppingCarts
				.FirstOrDefaultAsync(c => c.UserId == userId || c.GuestId == userId);

			if (cart == null)
			{
				throw new InvalidOperationException("Shopping cart not found.");
			}

			Checkout? checkout = await this
						.SingleOrDefaultAsync(c => c.Id == checkoutId);

			if (checkout == null)
			{
				throw new InvalidOperationException("Checkout not found.");
			}

			decimal subTotal = cart.ShoppingCartItems
				.Sum(item => item.TotalPrice);

			checkout.SubTotal = subTotal;
			await this.UpdateAsync(checkout);
		}

		public async Task UpdateCheckoutFromUserDefaultsAsync(int? checkoutId, ApplicationUser? user)
		{
			if ((checkoutId.HasValue) && (user != null))
			{
				Checkout? checkout = await this
						.GetByIdAsync(checkoutId.Value);

				if (checkout != null)
				{
					bool hasUserAnyDefaults = user.DefaultShippingAddressId.HasValue
										    || user.DefaultBillingAddressId.HasValue
											|| user.DefaultPaymentMethodId.HasValue
											|| user.DefaultPaymentDetailsId.HasValue;

					if (hasUserAnyDefaults)
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

						await this.UpdateAsync(checkout);
					}
				}
				else
				{
					throw new InvalidOperationException("Checkout not found.");
				}
			}
		}

		public async Task UpdateCheckoutFromLastOrderAsync(int? checkoutId, string? userId)
		{
			if ((checkoutId.HasValue) && (userId != null))
			{
				Checkout? checkout = await this
						.GetByIdAsync(checkoutId.Value);

				if (checkout != null)
				{
					ApplicationUser? user = await this._userManager
								.FindByIdAsync(userId);

					if (user != null)
					{
						if (IsCheckoutMissingEssentialData(checkout))
						{
							var lastOrder = await this._orderRepository
									.GetAllAttached()
									.Where(o => o.UserId == user.Id)
									.OrderByDescending(o => o.OrderDate)
									.FirstOrDefaultAsync();

							if (lastOrder != null)
							{
								SetCheckoutDefaultsFromOrder(checkout, lastOrder);
								await this.UpdateAsync(checkout);
							}
						}
					}
					else
					{
						if (IsCheckoutMissingEssentialData(checkout))
						{
							var lastOrder = await this._orderRepository
									.GetAllAttached()
									.Where(o => o.GuestId == userId)
									.OrderByDescending(o => o.OrderDate)
									.FirstOrDefaultAsync();

							if (lastOrder != null)
							{
								SetCheckoutDefaultsFromOrder(checkout, lastOrder);
								await this.UpdateAsync(checkout);
							}
						}
					}
				}
				else
				{
					throw new InvalidOperationException("Checkout not found.");
				}
			}
		}

		public async Task SetCheckoutDefaultPaymentMethodAsync(Checkout? checkout)
		{
			if (checkout != null)
			{
				if (checkout.PaymentMethodId <= 0 && checkout.PaymentMethod == null)
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
		}

		public void SetCheckoutDefaultShippingOption(Checkout? checkout, ApplicationUser? user)
		{
			if (checkout != null)
			{
				var today = DateTime.Today;

				var standardStart = AddBusinessDays(today, StandartShippingOptionDaysMin);
				var standardEnd = AddBusinessDays(today, StandartShippingOptionDaysMax);

				var isMember = user != null;

				decimal shippingPrice = 0;
				if (isMember)
				{
					shippingPrice = StandartShippingPriceForMembers;
				}
				else
				{
					shippingPrice = checkout.SubTotal >= MinPriceForFreeShipping
						? StandartShippingPriceForMembers
						: StandartShippingPriceForGuests;
				}

				string standartShippingOptionName = StandartShippingOptionName;

				checkout.ShippingOption = standartShippingOptionName;
				checkout.EstimatedDeliveryStart = standardStart;
				checkout.EstimatedDeliveryEnd = standardEnd;
				checkout.ShippingPrice = shippingPrice;
			}
			else
			{
				throw new InvalidOperationException("Checkout not found.");
			}
		}


		private static void SetCheckoutDefaultsFromOrder(Checkout checkout, Order lastOrder)
		{
			if (checkout.GuestId != null)
			{
				if (checkout.GuestName == null)
				{
					checkout.GuestName = lastOrder.GuestName;
				}

				if (checkout.GuestEmail == null)
				{
					checkout.GuestEmail = lastOrder.GuestEmail;
				}
			}

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

			if (checkout.PaymentMethodId <= 0 || int.IsNegative(checkout.PaymentMethodId))
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

		private static bool IsCheckoutMissingEssentialData(Checkout checkout)
		{
			bool result;
			if (checkout.UserId != null)
			{
				result = checkout.ShippingAddressId == null
							|| checkout.BillingAddressId == null
								|| checkout.PaymentMethodId <= 0;
			}
			else
			{
				result = checkout.GuestName == null
							|| checkout.GuestEmail == null
								|| checkout.ShippingAddressId == null
									|| checkout.BillingAddressId == null
										|| checkout.PaymentMethodId <= 0;
			}

			return result;
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

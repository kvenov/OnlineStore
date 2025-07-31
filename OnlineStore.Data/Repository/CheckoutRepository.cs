using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;

using static OnlineStore.Common.ApplicationConstants;

namespace OnlineStore.Data.Repository
{
	public class CheckoutRepository : BaseRepository<Checkout, int>, ICheckoutRepository
	{
		public CheckoutRepository(ApplicationDbContext dbContext) 
					: base(dbContext)
		{
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

			checkout.TotalPrice = subTotal;
			await this.SaveChangesAsync();
		}
	}
}

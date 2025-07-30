using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;
using System.Linq.Expressions;

using static OnlineStore.Common.ApplicationConstants;

namespace OnlineStore.Data.Repository
{
	public class ShoppingCartRepository : BaseRepository<ShoppingCart, int>, IShoppingCartRepository
	{
		public ShoppingCartRepository(ApplicationDbContext dbContext) : 
				base(dbContext)
		{
		}

		public async Task AddShoppingCartItemAsync(ShoppingCartItem item)
		{
			await this.DbContext.ShoppingCartsItems.AddAsync(item);
		}

		public IQueryable<ShoppingCartItem> GetAllShoppingCartItemsAttached()
		{
			return this.DbContext.ShoppingCartsItems
							.AsQueryable();
		}

		public async Task<decimal> GetItemsTotalPrice(string userId)
		{
			decimal totalPrice = 0;
			ShoppingCart? shoppingCart = await this.SingleOrDefaultAsync(
					sc => sc.UserId == userId || sc.GuestId == userId);

			if (shoppingCart != null)
			{
				foreach (var item in shoppingCart.ShoppingCartItems)
				{
					totalPrice += item.TotalPrice;
				}
			}

			return totalPrice;
		}

		public async Task<ShoppingCartItem?> GetShoppingCartItemAsync(Expression<Func<ShoppingCartItem, bool>> predicate)
		{
			return await this.DbContext.ShoppingCartsItems
						.SingleOrDefaultAsync(predicate);
		}

		public async Task<decimal> GetShoppingCartShippingCostByUserIdAsync(string? userId)
		{
			if (userId == null)
			{
				throw new InvalidCastException("User ID cannot be null.");
			}

			ApplicationUser? user = await this.DbContext.Users
						.SingleOrDefaultAsync(u => u.Id == userId);

			ShoppingCart? shoppingCart = await this
						.SingleOrDefaultAsync(sc => sc.UserId == userId || sc.GuestId == userId);

			if (shoppingCart == null)
			{
				throw new InvalidOperationException("Shopping cart not found.");
			}

			decimal subTotal = shoppingCart.ShoppingCartItems
							.Sum(item => item.TotalPrice);

			decimal deliveryCost = StandartShippingPriceForMembers;

			if (subTotal > 0)
			{
				if (user == null)
				{
					deliveryCost = subTotal >= MinPriceForFreeShipping ?
													StandartShippingPriceForMembers :
														StandartShippingPriceForGuests;
				}
			}

			return deliveryCost;
		}

		public void RemoveShoppingCartItem(ShoppingCartItem item)
		{
			this.DbContext.ShoppingCartsItems.Remove(item);
		}
	}
}

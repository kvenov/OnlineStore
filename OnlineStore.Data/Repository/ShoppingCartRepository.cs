using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;
using System.Linq.Expressions;

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

		public async Task<ShoppingCartItem?> GetShoppingCartItemAsync(Expression<Func<ShoppingCartItem, bool>> predicate)
		{
			return await this.DbContext.ShoppingCartsItems
						.SingleOrDefaultAsync(predicate);
		}

		public void RemoveShoppingCartItem(ShoppingCartItem item)
		{
			this.DbContext.ShoppingCartsItems.Remove(item);
		}
	}
}

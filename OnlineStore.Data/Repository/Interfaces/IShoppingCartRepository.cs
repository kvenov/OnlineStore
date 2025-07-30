using OnlineStore.Data.Models;
using System.Linq.Expressions;

namespace OnlineStore.Data.Repository.Interfaces
{
	public interface IShoppingCartRepository : IRepository<ShoppingCart, int>, IAsyncRepository<ShoppingCart, int>
	{

		Task<ShoppingCartItem?> GetShoppingCartItemAsync(Expression<Func<ShoppingCartItem, bool>> predicate);

		Task AddShoppingCartItemAsync(ShoppingCartItem item);

		IQueryable<ShoppingCartItem> GetAllShoppingCartItemsAttached();

		void RemoveShoppingCartItem(ShoppingCartItem item);

		Task<decimal> GetItemsTotalPrice(string userId);

		Task<decimal> GetShoppingCartShippingCostByUserIdAsync(string? userId);
	}
}

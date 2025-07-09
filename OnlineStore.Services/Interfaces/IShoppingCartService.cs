using OnlineStore.Web.ViewModels.Layout;
using OnlineStore.Web.ViewModels.ShoppingCart;

namespace OnlineStore.Services.Core.Interfaces
{
	public interface IShoppingCartService
	{

		Task<ShoppingCartViewModel?> GetShoppingCartForUserAsync(string? userId);

		Task<CartInfoViewModel?> GetUserShoppingCartDataAsync(string? userId);

		Task<int> GetUserShoppingCartItemsCountAsync(string? userId);

		Task<bool> AddToCartAsync(int? productId, string? userId);

		Task<ShoppingCartSummaryViewModel?> UpdateCartItemAsync(string? userId, int? quantity, int? itemId);

		Task<ShoppingCartSummaryViewModel?> RemoveCartItemAsync(string? userId, int? itemId);
	}
}

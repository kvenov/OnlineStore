using OnlineStore.Web.ViewModels.Layout;
using OnlineStore.Web.ViewModels.ShoppingCart;

namespace OnlineStore.Services.Core.Interfaces
{
	public interface IShoppingCartService
	{

		Task<ShoppingCartViewModel?> GetShoppingCartForUserAsync(string? userId);

		Task<CartInfoViewModel?> GetUserShoppingCartDataAsync(string? userId);

		Task<int> GetUserShoppingCartItemsCountAsync(string? userId);
	}
}

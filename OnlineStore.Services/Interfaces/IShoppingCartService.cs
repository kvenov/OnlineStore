using OnlineStore.Web.ViewModels.ShoppingCart;

namespace OnlineStore.Services.Core.Interfaces
{
	public interface IShoppingCartService
	{

		Task<ShoppingCartViewModel?> GetShoppingCartForUserAsync(string? userId);
	}
}

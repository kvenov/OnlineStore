using Microsoft.AspNetCore.Http;
using OnlineStore.Data.Models;
using OnlineStore.Web.ViewModels.Layout;
using OnlineStore.Web.ViewModels.ShoppingCart;

namespace OnlineStore.Services.Core.Interfaces
{
	public interface IShoppingCartService
	{

		Task<ShoppingCartViewModel?> GetShoppingCartForUserAsync(string? userId);

		Task<ShoppingCartViewModel?> GetShoppingCartForGuestAsync(string? guestId);

		Task<CartInfoViewModel?> GetUserShoppingCartDataAsync(string? userId);

		Task<CartInfoViewModel?> GetGuestShoppingCartDataAsync(string? guestId);

		Task<int> GetUserShoppingCartItemsCountAsync(string? userId);

		Task<int> GetGuestShoppingCartItemsCountAsync(string? guestId);

		Task<bool> AddToCartForUserAsync(int? productId, string? productSize, string? userId);

		Task<bool> AddToCartForGuestAsync(int? productId, string? productSize, string? guestId);

		Task<ShoppingCartSummaryViewModel?> UpdateUserCartItemAsync(string? userId, int? quantity, int? itemId);

		Task<ShoppingCartSummaryViewModel?> UpdateGuestCartItemAsync(string? guestId, int? quantity, int? itemId);

		Task<ShoppingCartSummaryViewModel?> RemoveUserCartItemAsync(string? userId, int? itemId);

		Task<ShoppingCartSummaryViewModel?> RemoveGuestCartItemAsync(string? guestId, int? itemId);

		Task AddNewShoppingCartAsync(ShoppingCart cart);
	}
}

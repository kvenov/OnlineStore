using OnlineStore.Web.ViewModels.Wishlist;

namespace OnlineStore.Services.Core.Interfaces
{
	public interface IWishlistService
	{

		Task<WishlistIndexViewModel> GetUserWishlist(string userId);

		Task<bool> AddProductToWishlist(int? productId, string userId);

		Task<bool> RemoveProductFromWishlistAsycn(int? itemId, string userId);

		Task<bool> EditNoteAsync(int? itemId, string? note, string userId);

		Task<int> GetWishlistItemsCountAsync(string userId);
	}
}

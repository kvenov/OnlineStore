using OnlineStore.Data.Models;

namespace OnlineStore.Data.Repository.Interfaces
{
	public interface IWishlistRepository : IRepository<Wishlist, int>, IAsyncRepository<Wishlist, int>
	{

		Task AddWishlistItemAsync(WishlistItem wishlistItem);

		Task<WishlistItem?> GetWishlistItemByIdAsync(int? id);

		IQueryable<WishlistItem> GetAllWishlistItemsAttached();

		void DeleteWishlistItem(WishlistItem item);
	}
}

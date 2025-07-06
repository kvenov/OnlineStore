using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;

namespace OnlineStore.Data.Repository
{
	public class WishlistRepository : BaseRepository<Wishlist, int>, IWishlistRepository
	{
		public WishlistRepository(ApplicationDbContext dbContext) 
					: base(dbContext)
		{
		}

		public async Task AddWishlistItemAsync(WishlistItem wishlistItem)
		{
			await this.DbContext.WishlistsItems.AddAsync(wishlistItem);
			await SaveChangesAsync();
		}

		public void DeleteWishlistItem(WishlistItem item)
		{
			this.DbContext.WishlistsItems.Remove(item);
		}

		public IQueryable<WishlistItem> GetAllWishlistItemsAttached()
		{
			return this.DbContext.WishlistsItems
					.AsQueryable();
		}

		public async Task<WishlistItem?> GetWishlistItemByIdAsync(int? id)
		{
			if (id != null)
			{
				return await this.DbContext.WishlistsItems
						.SingleOrDefaultAsync(wi => wi.Id == id);
			}

			return null;
		}

	}
}

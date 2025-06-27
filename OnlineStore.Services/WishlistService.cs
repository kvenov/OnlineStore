using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.Data.Models;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.Wishlist;
using OnlineStore.Web.ViewModels.Wishlist.Partial;

namespace OnlineStore.Services.Core
{
	public class WishlistService : IWishlistService
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public WishlistService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			this._context = context;
			this._userManager = userManager;
		}

		public async Task<bool> AddProductToWishlist(int? productId, string userId)
		{
			bool isAdded = false;

			if ((productId != null))
			{
				Product? product = await this._context
					.Products
					.FindAsync(productId);

				ApplicationUser? user = await this._userManager
					.FindByIdAsync(userId);

				if ((product != null) && (user != null))
				{
					Wishlist? wishlist = await this._context
					.Wishlists
					.Include(w => w.WishlistItems)
					.SingleOrDefaultAsync(w => w.UserId == user.Id);

					if (wishlist != null)
					{
						WishlistItem? wishlistItem = wishlist
						.WishlistItems
						.FirstOrDefault(wi => wi.ProductId == productId);

						if (wishlistItem == null)
						{
							WishlistItem newWishlistItem = new WishlistItem()
							{
								AddedAt = DateTime.UtcNow,
								WishlistId = wishlist.Id,
								ProductId = productId.Value,
							};

							wishlist.WishlistItems.Add(newWishlistItem);
							await this._context.WishlistsItems.AddAsync(newWishlistItem);
							await this._context.SaveChangesAsync();

							isAdded = true;
						}
					}

				}
			}

			return isAdded;
		}

		public async Task<bool> EditNoteAsync(int? itemId, string? note, string userId)
		{
			bool isEdited = false;

			if (itemId != null && note != null)
			{

				ApplicationUser? user = await this._userManager
						.FindByIdAsync(userId);

				if (user != null)
				{
					Wishlist? wishlist = await this._context
						.Wishlists
						.Include(w => w.WishlistItems)
						.SingleOrDefaultAsync(w => w.UserId == user.Id);

					if (wishlist != null)
					{

						WishlistItem? wishlistItem = await this._context
							.WishlistsItems
							.SingleOrDefaultAsync(wi => wi.Id == itemId);

						if (wishlistItem != null && wishlist.WishlistItems.Contains(wishlistItem))
						{
							wishlistItem.Notes = note;

							await this._context.SaveChangesAsync();
							isEdited = true;
						}
					}
				}
			}

			return isEdited;
		}

		public async Task<WishlistIndexViewModel> GetUserWishlist(string userId)
		{
			WishlistIndexViewModel wishlist = await this._context
					.Wishlists
					.AsNoTracking()
					.Include(w => w.WishlistItems)
					.ThenInclude(wi => wi.Product)
					.ThenInclude(p => p.Category)
					.Select(w => new WishlistIndexViewModel()
					{
						Id = w.Id,
						UserId = w.UserId,
						Items = w.WishlistItems
							.Select(wi => new WishlistItemPartialViewModel()
							{
								Id = wi.Id,
								ProductName = wi.Product.Name,
								ProductCategory = wi.Product.Category.Name,
								ImageUrl = wi.Product.ImageUrl,
								Price = wi.Product.Price,
								Notes = wi.Notes
							})
							.ToList()
					})
					.SingleAsync(w => w.UserId == userId);

			return wishlist;
		}

		public async Task<int> GetWishlistItemsCountAsync(string userId)
		{
			int count = 0;
			ApplicationUser? user = await this._userManager
				.FindByIdAsync(userId);

			if (user != null)
			{
				Wishlist? wishlist = await this._context
					.Wishlists
					.AsNoTracking()
					.Include(w => w.WishlistItems)
					.SingleOrDefaultAsync(w => w.UserId == userId);

				if (wishlist != null)
				{
					count = wishlist.WishlistItems.Count;
				}
			}

			return count;
		}

		public async Task<bool> RemoveProductFromWishlistAsycn(int? itemId, string userId) 
		{
			bool isRemoved = false;

			if (itemId != null)
			{
				ApplicationUser? user = await this._userManager
						.FindByIdAsync(userId);

				if (user != null)
				{
					Wishlist? wishlist = await this._context
						.Wishlists
						.Include(w => w.WishlistItems)
						.SingleOrDefaultAsync(w => w.UserId == user.Id);

					if (wishlist != null)
					{

						WishlistItem? wishlistItem = await this._context
							.WishlistsItems
							.SingleOrDefaultAsync(wi => wi.Id == itemId);

						if (wishlistItem != null && wishlist.WishlistItems.Contains(wishlistItem))
						{
							wishlist.WishlistItems.Remove(wishlistItem);
							this._context.WishlistsItems.Remove(wishlistItem);

							await this._context.SaveChangesAsync();
							isRemoved = true;
						}
					}
				}
			}

			return isRemoved;
		}
	}
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.Wishlist;
using OnlineStore.Web.ViewModels.Wishlist.Partial;

namespace OnlineStore.Services.Core
{
	public class WishlistService : IWishlistService
	{
		private readonly IProductRepository _productRepository;
		private readonly IWishlistRepository _wishlistRepository;
		private readonly UserManager<ApplicationUser> _userManager;

		public WishlistService(UserManager<ApplicationUser> userManager, 
							   IWishlistRepository wishlistRepository, 
							   IProductRepository productRepository)
		{
			this._userManager = userManager;
			this._wishlistRepository = wishlistRepository;
			this._productRepository = productRepository;
		}

		public async Task<bool> AddProductToWishlist(int? productId, string userId)
		{
			bool isAdded = false;

			if (productId != null)
			{
				Product? product = await this._productRepository
						.GetByIdAsync(productId.Value);

				ApplicationUser? user = await this._userManager
						.FindByIdAsync(userId);

				if ((product != null) && (user != null))
				{
					Wishlist? wishlist = await this._wishlistRepository
						.GetAllAttached()
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
							await this._wishlistRepository.AddWishlistItemAsync(newWishlistItem);

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
					Wishlist? wishlist = await this._wishlistRepository
						.GetAllAttached()
						.Include(w => w.WishlistItems)
						.SingleOrDefaultAsync(w => w.UserId == user.Id);

					if (wishlist != null)
					{

						WishlistItem? wishlistItem = await this._wishlistRepository
								.GetWishlistItemByIdAsync(itemId);

						if (wishlistItem != null && wishlist.WishlistItems.Contains(wishlistItem))
						{
							wishlistItem.Notes = note;

							await this._wishlistRepository.SaveChangesAsync();
							isEdited = true;
						}
					}
				}
			}

			return isEdited;
		}

		public async Task<WishlistIndexViewModel> GetUserWishlist(string userId)
		{
			WishlistIndexViewModel wishlist = await this._wishlistRepository
					.GetAllAttached()
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
				Wishlist? wishlist = await this._wishlistRepository
					.GetAllAttached()
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
					Wishlist? wishlist = await this._wishlistRepository
						.GetAllAttached()
						.Include(w => w.WishlistItems)
						.SingleOrDefaultAsync(w => w.UserId == user.Id);

					if (wishlist != null)
					{

						WishlistItem? wishlistItem = await this._wishlistRepository
							.GetAllWishlistItemsAttached()
							.IgnoreQueryFilters()
							.SingleOrDefaultAsync(wi => wi.Id == itemId);

						if (wishlistItem != null && wishlist.WishlistItems.Contains(wishlistItem))
						{
							try
							{
								wishlist.WishlistItems.Remove(wishlistItem);
								this._wishlistRepository.DeleteWishlistItem(wishlistItem);
								await this._wishlistRepository.SaveChangesAsync();
								isRemoved = true;
							}
							catch (DbUpdateConcurrencyException ex)
							{
								Console.WriteLine(ex.Message);

								isRemoved = false;
							}
						}
					}
				}
			}

			return isRemoved;
		}
	}
}

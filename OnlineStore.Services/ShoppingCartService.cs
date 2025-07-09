using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.Data.Models;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.Layout;
using OnlineStore.Web.ViewModels.ShoppingCart;

namespace OnlineStore.Services.Core
{
	public class ShoppingCartService : IShoppingCartService
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public ShoppingCartService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			this._context = context;
			this._userManager = userManager;
		}

		public async Task<CartInfoViewModel?> GetUserShoppingCartDataAsync(string? userId)
		{
			CartInfoViewModel? cartModel = null;

			if (userId != null)
			{
				ApplicationUser? user = await this._userManager
					.FindByIdAsync(userId);


				if (user != null)
				{

					cartModel = await this._context.ShoppingCarts
							.AsNoTracking()
							.Include(sc => sc.ShoppingCartItems)
							.ThenInclude(sci => sci.Product)
							.Where(sc => sc.UserId == user.Id)
							.Select(sc => new CartInfoViewModel()
							{
								Items = sc.ShoppingCartItems
											.Select(sci => new ShoppingCartItemViewModel()
											{
												Id = sci.Id,
												ProductId = sci.ProductId,
												ProductName = sci.Product.Name,
												ProductImageUrl = sci.Product.ImageUrl,
												UnitPrice = sci.Price,
												Quantity = sci.Quantity,
											})
											.ToList(),
								Total = sc.ShoppingCartItems.Sum(sci => sci.TotalPrice)
							})
							.FirstOrDefaultAsync();
				}
			}
			else
			{
				cartModel = new CartInfoViewModel();
			}

			return cartModel;
		}

		public async Task<ShoppingCartViewModel?> GetShoppingCartForUserAsync(string? userId)
		{
			ShoppingCartViewModel? cartModel = null;

			if (userId != null)
			{
				ApplicationUser? user = await this._userManager
					.FindByIdAsync(userId);


				if (user != null)
				{

					cartModel = await this._context.ShoppingCarts
							.AsNoTracking()
							.Include(sc => sc.ShoppingCartItems)
							.ThenInclude(sci => sci.Product)
							.Where(sc => sc.UserId == user.Id)
							.Select(sc => new ShoppingCartViewModel()
							{
								Items = sc.ShoppingCartItems
											.Select(sci => new ShoppingCartItemViewModel()
											{
												Id = sci.Id,
												ProductId = sci.ProductId,
												ProductName = sci.Product.Name,
												ProductImageUrl = sci.Product.ImageUrl,
												UnitPrice = sci.Price,
												Quantity = sci.Quantity,
											})
											.ToList()
							})
							.FirstOrDefaultAsync();
				}
			}
			else
			{
				cartModel = new ShoppingCartViewModel();
			}

			return cartModel;
		}

		public async Task<int> GetUserShoppingCartItemsCountAsync(string? userId)
		{
			if (userId != null)
			{

				ApplicationUser? user = await this._userManager
					.FindByIdAsync(userId);

				if (user != null)
				{

					ShoppingCart? shoppingCart = await this._context.ShoppingCarts
								.AsNoTracking()
								.Include(sc => sc.ShoppingCartItems)
								.SingleOrDefaultAsync(sc => sc.UserId == user.Id);

					if (shoppingCart != null)
					{
						return shoppingCart.ShoppingCartItems.Count;
					}
				}
			}

			return 0;
		}

		public async Task<bool> AddToCartAsync(int? productId, string? userId)
		{
			bool isAdded = false;

			if ((productId != null) && (userId != null))
			{
				Product? product = await this._context.Products
							.FindAsync(productId);

				ApplicationUser? user = await this._userManager
							.FindByIdAsync(userId);

				if ((product != null) && (user != null))
				{
					ShoppingCart? shoppingCart = await this._context.ShoppingCarts
								.Include(w => w.ShoppingCartItems)
								.SingleOrDefaultAsync(sc => sc.UserId == user.Id);

					if (shoppingCart != null)
					{
						ShoppingCartItem? existingShoppingCartItem = await this._context.ShoppingCartsItems
									.SingleOrDefaultAsync(sci => sci.ShoppingCartId == shoppingCart.Id && sci.ProductId == product.Id);

						if (existingShoppingCartItem != null)
						{
							existingShoppingCartItem.Quantity += 1;
						}
						else
						{
							int defaultProductQuantity = 1;
							decimal totalPrice = defaultProductQuantity * product.Price;

							ShoppingCartItem newItem = new ShoppingCartItem()
							{
								Quantity = defaultProductQuantity,
								Price = product.Price,
								TotalPrice = totalPrice,
								ShoppingCartId = shoppingCart.Id,
								ProductId = product.Id
							};

							await this._context.ShoppingCartsItems.AddAsync(newItem);
							shoppingCart.ShoppingCartItems.Add(newItem);
						}

						int affectedRows = await this._context.SaveChangesAsync();
						isAdded = affectedRows > 0;
					}
				}
			}


			return isAdded;
		}

		public async Task<ShoppingCartSummaryViewModel?> UpdateCartItemAsync(string? userId, int? quantity, int? itemId)
		{
			ShoppingCartSummaryViewModel? summaryModel = null;

			if ((itemId != null) && (userId != null) && (quantity != null))
			{
				ApplicationUser? user = await this._userManager
							.FindByIdAsync(userId);

				if (user != null)
				{
					ShoppingCart? shoppingCart = await this._context.ShoppingCarts
								.Include(w => w.ShoppingCartItems)
								.SingleOrDefaultAsync(sc => sc.UserId == user.Id);

					if (shoppingCart != null)
					{
						ShoppingCartItem? existingShoppingCartItem = await this._context.ShoppingCartsItems
										.Include(sci => sci.Product)
										.SingleOrDefaultAsync(sci => sci.Id == itemId);

						if (existingShoppingCartItem != null)
						{
							existingShoppingCartItem.Quantity = quantity.Value;
							existingShoppingCartItem.TotalPrice = existingShoppingCartItem.Price * quantity.Value;

							summaryModel = new ShoppingCartSummaryViewModel()
							{
								ItemTotalPrice = existingShoppingCartItem.TotalPrice,
								SubTotal = shoppingCart.ShoppingCartItems.Sum(sci => sci.TotalPrice)
							};
						}

						await this._context.SaveChangesAsync();
						
					}
				}
			}

			return summaryModel;
		}

		public async Task<bool> RemoveCartItemAsync(string? userId, int? itemId)
		{
			bool isRemoved = false;

			if ((itemId != null) && (userId != null))
			{
				ApplicationUser? user = await this._userManager
							.FindByIdAsync(userId);

				if (user != null)
				{
					ShoppingCart? shoppingCart = await this._context.ShoppingCarts
								.Include(w => w.ShoppingCartItems)
								.SingleOrDefaultAsync(sc => sc.UserId == user.Id);

					if (shoppingCart != null)
					{
						ShoppingCartItem? cartItemToRemove = await this._context.ShoppingCartsItems
										.Include(sci => sci.Product)
										.SingleOrDefaultAsync(sci => sci.Id == itemId);

						if (cartItemToRemove != null)
						{

							shoppingCart.ShoppingCartItems.Remove(cartItemToRemove);
							this._context.ShoppingCartsItems.Remove(cartItemToRemove);
						}

						int affectedRows = await this._context.SaveChangesAsync();
						isRemoved = affectedRows > 0;
					}
				}
			}

			return isRemoved;
		}

	}
}

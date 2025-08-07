using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.Layout;
using OnlineStore.Web.ViewModels.ShoppingCart;

namespace OnlineStore.Services.Core
{
	public class ShoppingCartService : IShoppingCartService
	{
		private readonly IShoppingCartRepository _shoppingCartRepository;
		private readonly IProductRepository _productRepository;
		private readonly UserManager<ApplicationUser> _userManager;

		public ShoppingCartService(UserManager<ApplicationUser> userManager,
								   IShoppingCartRepository shoppingCartRepository,
								   IProductRepository productRepository)
		{
			this._userManager = userManager;
			this._shoppingCartRepository = shoppingCartRepository;
			this._productRepository = productRepository;
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

					cartModel = await this._shoppingCartRepository
							.GetAllAttached()
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
												ProductSize = sci.ProductSize,
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
					decimal shipping = await this._shoppingCartRepository
							.GetShoppingCartShippingCostByUserIdAsync(user.Id);

					cartModel = await this._shoppingCartRepository
							.GetAllAttached()
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
												ProductSize = sci.ProductSize,
												UnitPrice = sci.Price,
												Quantity = sci.Quantity,
											})
											.ToList(),
								Shipping = shipping,
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

					ShoppingCart? shoppingCart = await this._shoppingCartRepository
								.GetAllAttached()
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

		//TODO: Refactor this method so that if the productSize is invalid for the current product category and for the valid app sizes!
		public async Task<bool> AddToCartForUserAsync(int? productId, string? productSize, string? userId)
		{
			bool isAdded = false;

			if ((productId != null) && (userId != null) && (productSize != null))
			{
				Product? product = await this._productRepository
							.GetByIdAsync(productId.Value);

				ApplicationUser? user = await this._userManager
							.FindByIdAsync(userId);

				if ((product != null) && (user != null))
				{
					ShoppingCart? shoppingCart = await this._shoppingCartRepository
								.GetAllAttached()
								.Include(w => w.ShoppingCartItems)
								.SingleOrDefaultAsync(sc => sc.UserId == user.Id);

					if (shoppingCart != null)
					{
						ShoppingCartItem? existingShoppingCartItem = await this._shoppingCartRepository
									.GetShoppingCartItemAsync(sci => sci.ShoppingCartId == shoppingCart.Id && sci.ProductId == product.Id);

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
								ProductId = product.Id,
								ProductSize = productSize
							};

							await this._shoppingCartRepository.AddShoppingCartItemAsync(newItem);
							shoppingCart.ShoppingCartItems.Add(newItem);
						}

						await this._shoppingCartRepository.SaveChangesAsync();
						isAdded = true;
					}
				}
			}
			
			return isAdded;
		}

		//TODO: Refactor this method so that if the productSize is invalid for the current product category and for the valid app sizes!
		public async Task<bool> AddToCartForGuestAsync(int? productId, string? productSize, string? guestId)
		{
			bool isAdded = false;

			if ((guestId != null) && (productId != null) && (productSize != null))
			{
				Product? product = await this._productRepository
							.GetByIdAsync(productId.Value);

				if (product != null)
				{
					ShoppingCart? shoppingCart = await this._shoppingCartRepository
								.GetAllAttached()
								.Include(w => w.ShoppingCartItems)
								.SingleOrDefaultAsync(sc => sc.GuestId == guestId);

					if (shoppingCart != null)
					{
						ShoppingCartItem? existingShoppingCartItem = await this._shoppingCartRepository
									.GetShoppingCartItemAsync(sci => sci.ShoppingCartId == shoppingCart.Id && sci.ProductId == product.Id);

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
								ProductId = product.Id,
								ProductSize = productSize
							};

							await this._shoppingCartRepository.AddShoppingCartItemAsync(newItem);
							shoppingCart.ShoppingCartItems.Add(newItem);
						}

						await this._shoppingCartRepository.SaveChangesAsync();
						isAdded = true;
					}
					else
					{
						ShoppingCart newShoppingCart = new ShoppingCart()
						{
							GuestId = guestId
						};

						await this._shoppingCartRepository.AddAsync(newShoppingCart);

						int defaultProductQuantity = 1;
						decimal totalPrice = defaultProductQuantity * product.Price;

						ShoppingCartItem newItem = new ShoppingCartItem()
						{
							Quantity = defaultProductQuantity,
							Price = product.Price,
							TotalPrice = totalPrice,
							ShoppingCartId = newShoppingCart.Id,
							ProductId = product.Id,
							ProductSize = productSize
						};

						await this._shoppingCartRepository.AddShoppingCartItemAsync(newItem);

						newShoppingCart.ShoppingCartItems.Add(newItem);

						await this._shoppingCartRepository.SaveChangesAsync();
						isAdded = true;
					}
				}
			}

			return isAdded;
		}

		public async Task<ShoppingCartSummaryViewModel?> UpdateUserCartItemAsync(string? userId, int? quantity, int? itemId)
		{
			ShoppingCartSummaryViewModel? summaryModel = null;

			if ((itemId != null) && (userId != null) && (quantity != null))
			{
				ApplicationUser? user = await this._userManager
							.FindByIdAsync(userId);

				if (user != null)
				{
					ShoppingCart? shoppingCart = await this._shoppingCartRepository
								.GetAllAttached()
								.Include(w => w.ShoppingCartItems)
								.SingleOrDefaultAsync(sc => sc.UserId == user.Id);

					if (shoppingCart != null)
					{
						ShoppingCartItem? existingShoppingCartItem = await this._shoppingCartRepository
										.GetAllShoppingCartItemsAttached()
										.Include(sci => sci.Product)
										.SingleOrDefaultAsync(sci => sci.Id == itemId);

						if (existingShoppingCartItem != null)
						{
							existingShoppingCartItem.Quantity = quantity.Value;
							existingShoppingCartItem.TotalPrice = existingShoppingCartItem.Price * quantity.Value;

							decimal shipping = await this._shoppingCartRepository
										.GetShoppingCartShippingCostByUserIdAsync(user.Id);

							summaryModel = new ShoppingCartSummaryViewModel()
							{
								ItemTotalPrice = existingShoppingCartItem.TotalPrice,
								SubTotal = shoppingCart.ShoppingCartItems.Sum(sci => sci.TotalPrice),
								Shipping = shipping
							};
						}

						await this._shoppingCartRepository.SaveChangesAsync();
					}
				}
			}

			return summaryModel;
		}

		public async Task<ShoppingCartSummaryViewModel?> RemoveUserCartItemAsync(string? userId, int? itemId)
		{
			ShoppingCartSummaryViewModel? summaryModel = null;

			if ((itemId != null) && (userId != null))
			{
				ApplicationUser? user = await this._userManager
							.FindByIdAsync(userId);

				if (user != null)
				{
					ShoppingCart? shoppingCart = await this._shoppingCartRepository
								.GetAllAttached()
								.Include(w => w.ShoppingCartItems)
								.SingleOrDefaultAsync(sc => sc.UserId == user.Id);

					if (shoppingCart != null)
					{
						ShoppingCartItem? cartItemToRemove = await this._shoppingCartRepository
										.GetAllShoppingCartItemsAttached()
										.Include(sci => sci.Product)
										.SingleOrDefaultAsync(sci => sci.Id == itemId);

						if (cartItemToRemove != null)
						{

							shoppingCart.ShoppingCartItems.Remove(cartItemToRemove);
							this._shoppingCartRepository.RemoveShoppingCartItem(cartItemToRemove);
						}

						await this._shoppingCartRepository.SaveChangesAsync();

						decimal shipping = await this._shoppingCartRepository
										.GetShoppingCartShippingCostByUserIdAsync(userId);

						summaryModel = new ShoppingCartSummaryViewModel()
						{
							SubTotal = shoppingCart.ShoppingCartItems.Sum(sci => sci.TotalPrice),
							Shipping = shipping
						};
					}
				}
			}

			return summaryModel;
		}

		public async Task<CartInfoViewModel?> GetGuestShoppingCartDataAsync(string? guestId)
		{
			CartInfoViewModel? cartModel = null;

			if (guestId != null)
			{
				cartModel = await this._shoppingCartRepository
						.GetAllAttached()
						.AsNoTracking()
						.Include(sc => sc.ShoppingCartItems)
						.ThenInclude(sci => sci.Product)
						.Where(sc => sc.GuestId == guestId)
						.Select(sc => new CartInfoViewModel()
						{
							Items = sc.ShoppingCartItems
										.Select(sci => new ShoppingCartItemViewModel()
										{
											Id = sci.Id,
											ProductId = sci.ProductId,
											ProductName = sci.Product.Name,
											ProductImageUrl = sci.Product.ImageUrl,
											ProductSize = sci.ProductSize,
											UnitPrice = sci.Price,
											Quantity = sci.Quantity,
										})
										.ToList(),
							Total = sc.ShoppingCartItems.Sum(sci => sci.TotalPrice)
						})
						.FirstOrDefaultAsync();
			}
			else
			{
				cartModel = new CartInfoViewModel();
			}

			return cartModel;
		}

		public async Task<int> GetGuestShoppingCartItemsCountAsync(string? guestId)
		{
			if (guestId != null)
			{

				ShoppingCart? shoppingCart = await this._shoppingCartRepository
							.GetAllAttached()
							.AsNoTracking()
							.Include(sc => sc.ShoppingCartItems)
							.SingleOrDefaultAsync(sc => sc.GuestId == guestId);

				if (shoppingCart != null)
				{
					return shoppingCart.ShoppingCartItems.Count;
				}
			}

			return 0;
		}

		public async Task<ShoppingCartViewModel?> GetShoppingCartForGuestAsync(string? guestId)
		{
			ShoppingCartViewModel? cartModel = null;

			if (guestId != null)
			{
				decimal shipping = await this._shoppingCartRepository
						.GetShoppingCartShippingCostByUserIdAsync(guestId);

				cartModel = await this._shoppingCartRepository
						.GetAllAttached()
						.AsNoTracking()
						.Include(sc => sc.ShoppingCartItems)
						.ThenInclude(sci => sci.Product)
						.Where(sc => sc.GuestId == guestId)
						.Select(sc => new ShoppingCartViewModel()
						{
							Items = sc.ShoppingCartItems
										.Select(sci => new ShoppingCartItemViewModel()
										{
											Id = sci.Id,
											ProductId = sci.ProductId,
											ProductName = sci.Product.Name,
											ProductImageUrl = sci.Product.ImageUrl,
											ProductSize = sci.ProductSize,
											UnitPrice = sci.Price,
											Quantity = sci.Quantity,
										})
										.ToList(),
							Shipping = shipping
						})
						.FirstOrDefaultAsync();
			}
			else
			{
				cartModel = new ShoppingCartViewModel();
			}

			return cartModel;
		}

		public async Task<ShoppingCartSummaryViewModel?> UpdateGuestCartItemAsync(string? guestId, int? quantity, int? itemId)
		{
			ShoppingCartSummaryViewModel? summaryModel = null;

			if ((itemId != null) && (guestId != null) && (quantity != null))
			{
				ShoppingCart? shoppingCart = await this._shoppingCartRepository
							.GetAllAttached()
							.Include(w => w.ShoppingCartItems)
							.SingleOrDefaultAsync(sc => sc.GuestId == guestId);

				if (shoppingCart != null)
				{
					ShoppingCartItem? existingShoppingCartItem = await this._shoppingCartRepository
									.GetAllShoppingCartItemsAttached()
									.Include(sci => sci.Product)
									.SingleOrDefaultAsync(sci => sci.Id == itemId);

					if (existingShoppingCartItem != null)
					{
						existingShoppingCartItem.Quantity = quantity.Value;
						existingShoppingCartItem.TotalPrice = existingShoppingCartItem.Price * quantity.Value;

						decimal shipping = await this._shoppingCartRepository
										.GetShoppingCartShippingCostByUserIdAsync(guestId);

						summaryModel = new ShoppingCartSummaryViewModel()
						{
							ItemTotalPrice = existingShoppingCartItem.TotalPrice,
							SubTotal = shoppingCart.ShoppingCartItems.Sum(sci => sci.TotalPrice),
							Shipping = shipping
						};
					}

					await this._shoppingCartRepository.SaveChangesAsync();
				}
			}

			return summaryModel;
		}

		public async Task<ShoppingCartSummaryViewModel?> RemoveGuestCartItemAsync(string? guestId, int? itemId)
		{
			ShoppingCartSummaryViewModel? summaryModel = null;

			if ((itemId != null) && (guestId != null))
			{

				ShoppingCart? shoppingCart = await this._shoppingCartRepository
							.GetAllAttached()
							.Include(w => w.ShoppingCartItems)
							.SingleOrDefaultAsync(sc => sc.GuestId == guestId);

				if (shoppingCart != null)
				{
					ShoppingCartItem? cartItemToRemove = await this._shoppingCartRepository
									.GetAllShoppingCartItemsAttached()
									.Include(sci => sci.Product)
									.SingleOrDefaultAsync(sci => sci.Id == itemId);

					if (cartItemToRemove != null)
					{

						shoppingCart.ShoppingCartItems.Remove(cartItemToRemove);
						this._shoppingCartRepository.RemoveShoppingCartItem(cartItemToRemove);
					}

					await this._shoppingCartRepository.SaveChangesAsync();

					decimal shipping = await this._shoppingCartRepository
										.GetShoppingCartShippingCostByUserIdAsync(guestId);

					summaryModel = new ShoppingCartSummaryViewModel()
					{
						SubTotal = shoppingCart.ShoppingCartItems.Sum(sci => sci.TotalPrice),
						Shipping = shipping
					};
				}
			}

			return summaryModel;
		}



		public async Task<ShoppingCart?> AddNewShoppingCartAsync(ApplicationUser? user)
		{
			if (user != null)
			{
				ShoppingCart newShoppingCart = new()
				{
					UserId = user.Id,
					User = user
				};

				await this._shoppingCartRepository.AddAsync(newShoppingCart);
				return newShoppingCart;
			}

			return null;
		}
	}
}

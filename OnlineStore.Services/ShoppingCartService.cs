using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.Data.Models;
using OnlineStore.Services.Core.Interfaces;
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
	}
}

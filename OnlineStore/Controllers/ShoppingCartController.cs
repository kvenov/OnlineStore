using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.Layout;
using OnlineStore.Web.ViewModels.ShoppingCart;

namespace OnlineStore.Web.Controllers
{
	public class ShoppingCartController : BaseController
	{
		private readonly IShoppingCartService _shoppingCartService;

		public ShoppingCartController(IShoppingCartService shoppingCartService)
		{
			this._shoppingCartService = shoppingCartService;
		}

		[AllowAnonymous]
		public async Task<IActionResult> Index()
		{
			try
			{
				string? userId = this.GetUserId();

				ShoppingCartViewModel? model = await _shoppingCartService
							.GetShoppingCartForUserAsync(userId);

				if (model == null)
				{
					return this.RedirectToAction(nameof(Index), "Home");
				}

				return View(model);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return this.RedirectToAction(nameof(Index), "Home");
			}
		}

		[HttpGet("cartFlyout")]
		public async Task<IActionResult> GetCartFlyoutDataPartial()
		{
			try
			{
				string? userId = this.GetUserId();

				CartInfoViewModel? model = await this._shoppingCartService
												.GetUserShoppingCartDataAsync(userId);

				if (model != null)
				{
					return PartialView("_CartFlyoutPartial", model);
				}

				return BadRequest();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return BadRequest("Something went wrong!");
			}
		}
	}
}

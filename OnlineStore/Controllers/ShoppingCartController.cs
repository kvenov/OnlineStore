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

				ShoppingCartViewModel? model;
				if (userId != null)
				{
					model = await _shoppingCartService
								.GetShoppingCartForUserAsync(userId);
				}
				else
				{
					string? guestId = this.HttpContext.Items["GuestIdentifier"]?.ToString();
					model = await this._shoppingCartService
								.GetShoppingCartForGuestAsync(guestId);
				}

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

		[AllowAnonymous]
		[HttpGet("cartFlyout")]
		public async Task<IActionResult> GetCartFlyoutDataPartial()
		{
			try
			{
				string? userId = this.GetUserId();

				CartInfoViewModel? model;
				if (userId != null)
				{
					model = await this._shoppingCartService
											.GetUserShoppingCartDataAsync(userId);
				}
				else
				{
					string? guestId = this.HttpContext.Items["GuestIdentifier"]?.ToString();
					model = await this._shoppingCartService
											.GetGuestShoppingCartDataAsync(guestId);
				}

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

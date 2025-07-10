using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.Layout;
using System.Security.Claims;

namespace OnlineStore.Web.ViewComponents
{
	public class CartFlyoutViewComponent : ViewComponent
	{

		private readonly IShoppingCartService _shoppingCartService;

		public CartFlyoutViewComponent(IShoppingCartService shoppingCartService)
		{
			this._shoppingCartService = shoppingCartService;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			string? userId = HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			CartInfoViewModel? model;
			if (userId != null)
			{
				model = await _shoppingCartService
							.GetUserShoppingCartDataAsync(userId);
			}
			else
			{
				string? guestId = this.HttpContext?.Items["GuestIdentifier"]?.ToString();
				model = await _shoppingCartService
							.GetGuestShoppingCartDataAsync(guestId);
			}
			

			return View("Default", model);
		}
	}
}

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
			CartInfoViewModel? model = await _shoppingCartService.GetUserShoppingCartDataAsync(userId);

			return View("Default", model);
		}
	}
}

using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.Layout;
using System.Security.Claims;

namespace OnlineStore.Web.ViewComponents
{
	public class CartSummaryViewComponent : ViewComponent
	{

		private readonly IShoppingCartService _shoppingCartService;

		public CartSummaryViewComponent(IShoppingCartService shoppingCartService)
		{
			this._shoppingCartService = shoppingCartService;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			string? userId = HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
			int itemsCount = await _shoppingCartService.GetUserShoppingCartItemsCountAsync(userId);

			var summary = new CartSummaryViewModel
			{
				CartItemsCount = itemsCount,
			};

			return View(summary);
		}
	}
}

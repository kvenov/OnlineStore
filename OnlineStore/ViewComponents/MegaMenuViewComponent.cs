using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.Layout;

namespace OnlineStore.Web.ViewComponents
{
	public class MegaMenuViewComponent : ViewComponent
	{
		private readonly IProductCategoryService _productCategoryService;

		public MegaMenuViewComponent(IProductCategoryService productCategoryService)
		{
			this._productCategoryService = productCategoryService;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{

			IEnumerable<MenuViewModel> menuModel = await this._productCategoryService
									.GetLayoutCategoryMenuAsync();

			return View("Default", menuModel);
		}
	}
}

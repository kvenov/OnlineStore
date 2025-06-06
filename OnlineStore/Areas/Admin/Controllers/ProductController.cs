using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Core.Admin.Interfaces;
using OnlineStore.Web.ViewModels.Admin.Product;

namespace OnlineStore.Web.Areas.Admin.Controllers
{
	public class ProductController : Controller
	{
		private readonly IAdminProductService _productService;

		public ProductController(IAdminProductService productService)
		{
			this._productService = productService;
		}


		[Area("Admin")]
		[HttpGet]
		[Authorize(Roles = "Admin")]
		[ActionName("All")]
		public async Task<IActionResult> All()
		{
			IEnumerable<AllProductsViewModel> model = await _productService.GetAllProductsAsync();

			return View(model);
		}


	}
}

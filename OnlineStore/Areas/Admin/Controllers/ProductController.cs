using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Core.Admin.Interfaces;
using OnlineStore.Web.ViewModels.Admin.Product;

namespace OnlineStore.Web.Areas.Admin.Controllers
{
	public class ProductController : Controller
	{
		private readonly IAdminProductService _productService;
		private readonly IAdminProductCategoryService _productCategoryService;
		private readonly IAdminBrandService _brandService;

		public ProductController(IAdminProductService productService,
								 IAdminProductCategoryService productCategoryService,
								 IAdminBrandService brandService)
		{
			this._productService = productService;
			this._productCategoryService = productCategoryService;
			this._brandService = brandService;
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

		[Area("Admin")]
		[HttpGet]
		[Authorize(Roles = "Admin")]
		[ActionName("Add")]
		public async Task<IActionResult> Add()
		{
			AddProductViewModel model = new AddProductViewModel
			{
				Categories = await _productCategoryService.GetAllProductCategoriesIdsAndNamesAsync(),
				Brands = await _brandService.GetAllBrandsIdsAndNamesAsync()
			};

			return View(model);
		}

		[Area("Admin")]
		[Authorize(Roles = "Admin")]
		[ActionName("Add")]
		[HttpPost]
		public async Task<IActionResult> Add(AddProductViewModel model)
		{
			if (!this.ModelState.IsValid)
			{
				return View(model);
			}

			try
			{
				await this._productService.AddProductAsync(model);

				return RedirectToAction(nameof(All), "Product", new { Area = "Admin" });
			}
			catch (Exception ex)
			{
				this.ModelState.AddModelError(string.Empty, ex.Message);
				return View(model);
			}
		}
	}
}

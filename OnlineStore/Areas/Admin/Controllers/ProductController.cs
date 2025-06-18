using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Core.Admin.Interfaces;
using OnlineStore.Web.ViewModels.Admin.Product;

namespace OnlineStore.Web.Areas.Admin.Controllers
{

	[Area("Admin")]
	[Authorize(Roles = "Admin")]
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


		[HttpGet]
		[ActionName("All")]
		public async Task<IActionResult> All()
		{
			try
			{
				IEnumerable<AllProductsViewModel> model = await _productService.GetAllProductsAsync();

				if (model == null)
				{
					return this.RedirectToAction("Index", "Dashboard");
				}

				return View(model);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return this.RedirectToAction("Index", "Dashboard");
			}
		}

		[HttpGet]
		[ActionName("Details")]
		public async Task<IActionResult> Details(int? id)
		{
			//TODO: Implement product details view and logic
			return RedirectToAction(nameof(All));
		}

		[HttpGet]
		[ActionName("Add")]
		public async Task<IActionResult> Add()
		{
			AddProductInputModel model = new AddProductInputModel
			{
				Categories = await _productCategoryService.GetAllProductCategoriesIdsAndNamesAsync(),
				Brands = await _brandService.GetAllBrandsIdsAndNamesAsync()
			};

			return View(model);
		}

		[HttpPost]
		[ActionName("Add")]
		public async Task<IActionResult> Add(AddProductInputModel model)
		{
			try
			{
				if (!this.ModelState.IsValid)
				{
					this.ModelState.AddModelError(string.Empty, "Invalid data. Please check the form and try again.");

					return View(model);
				}

				bool isAdded = await this._productService.AddProductAsync(model);

				if (isAdded)
				{
					return RedirectToAction(nameof(All));
				}
				else
				{
					this.ModelState.AddModelError(string.Empty, "Product addition failed. Please try again.");

					model.Categories = await _productCategoryService.GetAllProductCategoriesIdsAndNamesAsync();
					model.Brands = await _brandService.GetAllBrandsIdsAndNamesAsync();

					return View(model);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return View(model);
			}
		}

		[HttpGet]
		[ActionName("Edit")]
		public async Task<IActionResult> Edit(int? id)
		{
			try
			{
				if (id == null)
				{
					return RedirectToAction(nameof(All));
				}

				EditProductInputModel? model = await this._productService.GetEditableProductByIdAsync(id);

				if (model == null)
				{
					return RedirectToAction(nameof(All));
				}

				model.Categories = await this._productCategoryService.GetAllProductCategoriesIdsAndNamesAsync();
				model.Brands = await this._brandService.GetAllBrandsIdsAndNamesAsync();

				return View(model);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return RedirectToAction(nameof(All));
			}
		}

		[HttpPost]
		[ActionName("Edit")]
		public async Task<IActionResult> Edit(EditProductInputModel? model)
		{
			try
			{
				if (!this.ModelState.IsValid)
				{
					this.ModelState.AddModelError(string.Empty, "Invalid data. Please check the form and try again.");
					
					return View(model);
				}

				if (model == null)
				{
					return RedirectToAction(nameof(All));
				}

				bool isEdited = await this._productService.EditProductAsync(model);

				if (isEdited)
				{
					string refererUrl = this.HttpContext.Request.Headers.Referer.ToString();
					if (refererUrl.Contains("Details"))
					{
						return this.RedirectToAction(nameof(Details), new { id = model.Id });
					}
					else
					{
						return RedirectToAction(nameof(All));
					}
				}
				else
				{
					return this.RedirectToAction(nameof(All));
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return RedirectToAction(nameof(All));
			}
		}

		[HttpGet]
		[ActionName("Delete")]
		public async Task<IActionResult> Delete(string? id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				return this.RedirectToAction(nameof(All));
			}

			ProductDetailsForDeleteViewModel? model = await this._productService.GetProductDetailsForDeleteAsync(id);

			if (model == null)
			{
				return this.RedirectToAction(nameof(All));
			}

			return View(model);
		}

		[HttpPost]
		[ActionName("Delete")]
		public async Task<IActionResult> DeleteConfirmed(string? id)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(id))
				{
					return RedirectToAction(nameof(All));
				}

				bool isDeleted = await this._productService.SoftDeleteProductAsync(id);

				if (isDeleted)
				{
					return RedirectToAction(nameof(All));
				}
				else
				{
					this.ModelState.AddModelError(string.Empty, "Product deletion failed. Please try again.");

					return RedirectToAction(nameof(Delete), "Product", new { id });
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return RedirectToAction(nameof(All));
			}
		}
	}
}

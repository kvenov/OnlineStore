using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.Product;

namespace OnlineStore.Web.Controllers
{
	public class ProductController : BaseController
	{
		private readonly IProductService _productService;

		public ProductController(IProductService service)
		{
			this._productService = service;
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> ProductList()
		{
			try
			{
				IEnumerable<AllProductListViewModel> productList = await this._productService
						.GetAllProductsAsync();

				if (productList == null)
				{
					return RedirectToAction("Index", "Home");
				}

				return View(productList);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return RedirectToAction("Index", "Home");
			}
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Details(int? id)
		{
			try
			{
				ProductDetailsViewModel? productDetails = await this._productService
							.GetProductDetailsByIdAsync(id);

				if (productDetails == null)
				{
					return NotFound();
				}

				return View(productDetails);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return RedirectToAction("Index", "Home");
			}
		}
	}
}

using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.Product;

namespace OnlineStore.Web.Controllers
{
	public class ProductController : Controller
	{
		private readonly IProductService _productService;

		public ProductController(IProductService service)
		{
			this._productService = service;
		}

		[HttpGet]
		[ActionName("ProductList")]
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
	}
}

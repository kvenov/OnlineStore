using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineStore.Models;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.Controllers;
using OnlineStore.Web.ViewModels.Home;
using OnlineStore.Web.ViewModels.Product;

namespace OnlineStore.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IBrandService _brandService;

		public HomeController(ILogger<HomeController> logger, 
                              IProductService productService, 
                              IProductCategoryService productCategoryService, 
                              IBrandService brandService)
        {
            this._logger = logger;
            this._productService = productService;
            this._productCategoryService = productCategoryService;
            this._brandService = brandService;
		}

        [HttpGet]
        [AllowAnonymous]
		public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<AllProductListViewModel> productList = await this._productService
                            .GetAllProductsAsync();

                IEnumerable<SelectListItem> categories = await this._productCategoryService
                            .GetAllProductCategoriesIdsAndNamesAsync();

                IEnumerable<SelectListItem> brands = await this._brandService
                            .GetAllBrandsIdsAndNamesAsync();

				if ((productList == null) || (categories == null) || (brands == null))
				{
                    this._logger.LogWarning("One or more of the required data sets (products, categories, brands) are null.");

                    return RedirectToAction(nameof(Error));
				}

                IEnumerable<AllProductListViewModel> topNineProducts = productList
                    .OrderByDescending(p => p.Rating)
                    .Take(9);

				HomeIndexViewModel model = new HomeIndexViewModel()
                {
                    Products = topNineProducts,
                    Categories = categories,
                    Brands = brands
				};

				if (productList == null)
                {
                    this._logger.LogWarning("No products found in the database.");
                    
					return RedirectToAction(nameof(Error));
				}

                return View(model);
			}
            catch (Exception ex)
            {
                this._logger.LogError(ex, "An error occurred while loading the home page.");

                return RedirectToAction(nameof(Error));
			}
		}

        [HttpGet]
        [AllowAnonymous]
		public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult About()
        {
            ViewBag.Message = "Your application description page.";
			return View();
		}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

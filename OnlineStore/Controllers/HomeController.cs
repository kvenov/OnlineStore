using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Models;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.Controllers;
using OnlineStore.Web.ViewModels.Home;
using OnlineStore.Web.ViewModels.Home.Partial;

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
                HomeIndexViewModel model = new HomeIndexViewModel();
                IEnumerable<TrendingProductViewModel> trendings = await this._productService
                            .GetBestProductsAsync();

                model.Trendings = trendings;

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

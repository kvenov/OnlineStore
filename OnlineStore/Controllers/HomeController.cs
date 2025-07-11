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
        private readonly Dictionary<int, string> statusCodesCache = new Dictionary<int, string>
        {
            {401, "UnauthorizedError"}
        };

		private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly IProductPromotionService _productPromotionService;
        private readonly IArticleService _articleService;

		public HomeController(ILogger<HomeController> logger, 
                              IProductService productService, 
                              IProductPromotionService productPromotionService,
                              IArticleService articleService)
        {
            this._logger = logger;
            this._productService = productService;
            this._productPromotionService = productPromotionService;
            this._articleService = articleService;
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

                IEnumerable<ProductPromotionViewModel> promotions = await this._productPromotionService
                            .GetProductsPromotionsAsync();

                IEnumerable<UserReviewViewModel> articles = await this._articleService
                            .GetUserReviewsAsync();

                model.Trendings = trendings;
                model.Promotions = promotions;
                model.Reviews = articles;

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

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode)
        {

            switch (statusCode){
                case 401:
                    return View("UnauthorizedError");
                case 404:
                    return View("NotFoundError");
				case 403:
					return View("NotFoundError");
				default:
					return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
			}
        }
    }
}

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Models;

namespace OnlineStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
			//This redirect to the Product controller's Index action.
            //Fix this!!!
			return RedirectToAction("Index", "Product");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        [ActionName("About")]
        [RequireHttps]
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

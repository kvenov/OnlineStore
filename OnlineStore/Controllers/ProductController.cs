using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Core.Interfaces;

namespace OnlineStore.Web.Controllers
{
	public class ProductController(IProductService service) : Controller
	{

		[HttpGet]
		[ActionName("Index")]
		[RequireHttps]
		public async Task<IActionResult> Index()
		{
			var models = await service.GetAllProductsAsync();

			return View(models);
		}
	}
}

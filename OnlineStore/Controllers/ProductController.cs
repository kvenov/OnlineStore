using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Core.Interfaces;

namespace OnlineStore.Web.Controllers
{
	public class ProductController : Controller
	{
		private readonly IProductService _service;

		public ProductController(IProductService service)
		{
			this._service = service ?? throw new ArgumentNullException(nameof(service));
		}

		[HttpGet]
		[ActionName("Index")]
		[RequireHttps]
		public async Task<IActionResult> Index()
		{
			var models = await _service.GetAllProductsAsync();

			return View(models);
		}
	}
}

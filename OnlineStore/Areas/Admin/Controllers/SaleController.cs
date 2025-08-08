using Microsoft.AspNetCore.Mvc;

namespace OnlineStore.Web.Areas.Admin.Controllers
{
	public class SaleController : BaseAdminController
	{

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public IActionResult Overview()
		{
			return View();
		}
	}
}

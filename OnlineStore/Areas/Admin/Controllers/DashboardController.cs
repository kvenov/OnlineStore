using Microsoft.AspNetCore.Mvc;

namespace OnlineStore.Web.Areas.Admin.Controllers
{
	public class DashboardController : BaseAdminController
	{

		public IActionResult ProductIndex()
		{
			return View();
		}
	}
}

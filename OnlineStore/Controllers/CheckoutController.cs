using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Web.ViewModels.Checkout;

namespace OnlineStore.Web.Controllers
{
	public class CheckoutController : BaseController
	{

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Index()
		{
			CheckoutViewModel model = new CheckoutViewModel();

			return View(model);
		}
	}
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Web.Infrastructure.Filters;
using OnlineStore.Web.ViewModels.Checkout;

namespace OnlineStore.Web.Controllers
{
	public class OrderController : BaseController
	{

		[HttpPost]
		[AllowAnonymous]
		[ServiceFilter(typeof(CleanEmptyAddressFilter))]
		public Task<IActionResult> CreateOrder([FromForm]CheckoutViewModel model)
		{
			throw new NotImplementedException();
		}
	}
}

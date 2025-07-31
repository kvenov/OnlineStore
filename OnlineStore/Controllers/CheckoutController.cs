using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Data.Models;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.Checkout;

namespace OnlineStore.Web.Controllers
{
	public class CheckoutController : BaseController
	{
		private readonly ICheckoutService _checkoutService;

		public CheckoutController(ICheckoutService checkoutService)
		{
			this._checkoutService = checkoutService;
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Index()
		{
			try
			{
				string? userId = null;

				if (this.IsAuthenticated())
				{
					userId = this.GetUserId();
				}
				else
				{
					userId = this.GetGuestId();
				}

				Checkout? checkout = await this._checkoutService
									.InitializeCheckoutAsync(userId);

				if (checkout == null)
				{
					return NotFound(new
					{
						Error = "Checkout not found.",
						Details = "The checkout could not be initialized. Please try again later."
					});
				}

				CheckoutViewModel? model = await this._checkoutService
						.GetUserCheckout(checkout);

				if (model == null)
				{
					return BadRequest();
				}

				return View(model);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				
				return BadRequest(new
				{
					Error = "An error occurred while processing your request.",
					Details = ex.Message
				});
			}
		}
	}
}

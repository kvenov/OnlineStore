using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.Checkout;

using static OnlineStore.Common.ApplicationConstants;

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

				CheckoutViewModel? model = await this._checkoutService
							.GetUserCheckoutAsync(userId);

				if (model == null)
				{
					return BadRequest();
				}

				if (model.IsGuest)
				{
					decimal totalPrice = model.Summary.Subtotal;
					model.Summary.DeliveryCost = totalPrice >= MinPriceForFreeShipping ?
									StandartShippingPriceForMembers :
											StandartShippingPriceForGuests;
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

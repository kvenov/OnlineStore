using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Data.Models;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.Infrastructure.Filters;
using OnlineStore.Web.ViewModels.Checkout;
using OnlineStore.Web.ViewModels.Order;

namespace OnlineStore.Web.Controllers
{
	public class OrderController : BaseController
	{
		private readonly ILogger<OrderController> _logger;
		private readonly ICheckoutService _checkoutService;
		private readonly IOrderService _orderService;

		public OrderController(ILogger<OrderController> logger, 
							  ICheckoutService checkoutService,
							  IOrderService orderService)
		{
			this._logger = logger;
			this._checkoutService = checkoutService;
			this._orderService = orderService;
		}

		[AllowAnonymous]
		[HttpPost]
		[ServiceFilter(typeof(CleanEmptyAddressFilter))]
		[ServiceFilter(typeof(CleanInvalidPaymentDetailsFilter))]
		public async Task<IActionResult> CreateOrder([FromForm] CheckoutViewModel model)
		{
			if (!this.ModelState.IsValid)
			{
				this._logger.LogError("Model state is invalid.");

				return this.View(model);
			}

			try
			{

				Checkout? checkout = null;
				if (this.IsAuthenticated())
				{
					checkout = await this._checkoutService
							.UpdateUserCheckoutAsync(model);
				}
				else
				{
					checkout = await this._checkoutService
							.UpdateGuestCheckoutAsync(model);
				}

				if (checkout == null)
				{
					this._logger.LogWarning("Checkout not found or could not be updated.");

					return NotFound();
				}

				int? newOrderId = await this._orderService
							.CreateOrderAsync(checkout);

				if (newOrderId == null)
				{
					this._logger.LogError("Order could not be created.");

					return BadRequest();
				}

				string userId;
				if (this.IsAuthenticated())
				{
					userId = this.GetUserId()!;
				}
				else
				{
					userId = GetGuestId()!;
				}

				OrderConfirmationViewModel? viewModel = await this._orderService
									.GetOrderForConfirmationPageAsync(userId, newOrderId);

				if (viewModel == null)
				{
					this._logger.LogWarning("View Model for new Order not found");

					return NotFound();
				}

				return View("ThankYou", viewModel);
			}
			catch (Exception ex)
			{
				this._logger.LogError(ex, "An error occurred while creating the order.");

				return this.RedirectToAction("Index", "Home" );
			}
		}
	}
}

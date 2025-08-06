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

		[HttpGet]
		[ActionName("All")]
		public async Task<IActionResult> GetUserOrders()
		{
			try
			{
				string userId = this.GetUserId()!;

				if (userId == null)
				{
					return BadRequest();
				}

				IEnumerable<UserOrderViewModel>? orders = await this._orderService
									.GetUserOrdersAsync(userId);

				if (orders == null)
				{
					return NotFound();
				}

				return View(orders);
			}
			catch (Exception ex)
			{
				this._logger.LogError(ex, "An Error occured while getting the User Orders!");

				return this.RedirectToAction("Index", "Home");
			}
		}

		[AllowAnonymous]
		[HttpGet]
		[ActionName("GuestOrder")]
		public IActionResult GetGuestOrder()
		{
			TrackGuestOrderViewModel model = new TrackGuestOrderViewModel();

			return View(model);
		}

		[AllowAnonymous]
		[HttpPost]
		[ActionName("GuestOrder")]
		public async Task<IActionResult> GetGuestOrder(TrackGuestOrderViewModel? model)
		{
			if (!ModelState.IsValid)
			{
				this.ModelState.AddModelError(string.Empty, "The passed arguments are invalid or doesn't meet the system requirments!");

				return View(model);
			}

			try
			{
				string? guestId = this.GetGuestId();

				if (guestId == null)
				{
					return this.BadRequest();
				}

				bool result = await this._orderService
							.GetGuestOrderAsync(guestId, model);

				if (!result)
				{
					return View(model);
				}

				return View(model);
			}
			catch (Exception ex)
			{
				this._logger.LogError(ex, "Something went wrong while getting the guest order");

				return this.RedirectToAction("Index", "Home");
			}
		}

		[AllowAnonymous]
		[HttpGet]
		public async Task<IActionResult> Details(string? orderNumber)
		{
			try
			{
				OrderDetailsViewModel? details = await this._orderService
									.GetOrderDetailsAsync(orderNumber);

				if (details == null)
				{
					return NotFound();
				}

				return View(details);

			}
			catch (Exception ex)
			{
				this._logger.LogError(ex, "An error occured while getting the Order details");

				return this.RedirectToAction("Index", "Home");
			}
		}
	}
}

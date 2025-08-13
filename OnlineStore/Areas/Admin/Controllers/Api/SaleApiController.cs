using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Core.Admin.Interfaces;
using OnlineStore.Services.Core.DTO.Sales.CustomerInsights;
using OnlineStore.Services.Core.DTO.Sales.LocationSale;
using OnlineStore.Services.Core.DTO.Sales.OrderManagement;
using OnlineStore.Services.Core.DTO.Sales.Overview;
using OnlineStore.Services.Core.DTO.Sales.ProductAnalytics;
using OnlineStore.Web.ViewModels.Admin.Sale.LocationSales;
using OnlineStore.Web.ViewModels.Admin.Sale.ProductAnalytics;

namespace OnlineStore.Web.Areas.Admin.Controllers.Api
{
	public class SaleApiController : BaseAdminApiController
	{
		private readonly IAdminSaleService _saleService;
		private readonly ILogger<SaleApiController> _logger;

		public SaleApiController(IAdminSaleService saleService, ILogger<SaleApiController> logger)
		{
			this._saleService = saleService;
			this._logger = logger;
		}

		[HttpGet("overview")]
		public async Task<IActionResult> GetSalesOverview(string? range)
		{
			DateTime startDate;
			DateTime endDate = DateTime.UtcNow;

			switch (range?.ToLower())
			{
				case "daily":
					startDate = endDate.Date;
					break;
				case "weekly":
					startDate = endDate.Date.AddDays(-7);
					break;
				case "monthly":
					startDate = endDate.Date.AddMonths(-1);
					break;
				case "yearly":
					startDate = endDate.Date.AddYears(-1);
					break;
				default:
					startDate = endDate.Date.AddDays(-7);
					break;
			}

			try
			{
				AdminSalesOverviewViewModel? model = await this._saleService
										.GetSaleOverviewAsync(startDate, endDate);

				if (model == null)
				{
					return BadRequest();
				}

				return Ok(model);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Something went wrong while getting the Sale Overview!");

				return BadRequest();
			}
		}

		[HttpGet("product-analytics")]
		public async Task<IActionResult> GetProductsAnalytics([FromQuery] ProductAnalyticsFilterDto? dto)
		{
			try
			{
				ProductAnalyticsViewModel model = await this._saleService
											.GetProductAnalyticsAsync(dto);

				return Ok(model);
			}
			catch (Exception ex)
			{
				this._logger.LogError(ex, "Something went wrong while getting the ProductAnalytics");

				return BadRequest();
			}
		}

		[HttpGet("sales-by-location")]
		public async Task<IActionResult> GetSalesByLocation([FromQuery] SalesByLocationDto dto)
		{
			try
			{
				LocationSalesViewModel model = await this._saleService
											.GetSalesByLocationAsync(dto);

				return Ok(model);
			}
			catch (Exception ex)
			{
				this._logger.LogError(ex, "Something went wrong while getting the location sales!");

				return BadRequest();
			}
		}

		[HttpGet("filter")]
		public async Task<IActionResult> GetFilteredOrders([FromQuery] OrderFilterDto? dto)
		{
			try
			{
				IEnumerable<OrderListItemViewModel>? model = await this._saleService
										.GetFilteredOrdersAsync(dto);

				if (model == null)
				{
					return BadRequest();
				}

				return Ok(model);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Something went wrong while getting the filtered orders!");

				return BadRequest();
			}
		}

		[HttpGet("details/{orderId}")]
		public async Task<IActionResult> GetOrderDetails(int? orderId)
		{
			try
			{
				OrderDetailsViewModel? model = await this._saleService
										.GetOrderDetailsAsync(orderId);

				if (model == null)
				{
					return BadRequest();
				}

				return Ok(model);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Something went wrong while getting the order details!");

				return BadRequest();
			}
		}

		[HttpPost("cancel/{orderId}")]
		[AutoValidateAntiforgeryToken]
		public async Task<IActionResult> CancleOrder(int? orderId)
		{
			try
			{
				(bool isCancelled, string message) = await this._saleService
									.CancelOrderAsync(orderId);

				return Ok(new
				{
					result = isCancelled,
					message = message
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Something went wrong while canceling the Order!");

				return BadRequest(new
				{
					result = false,
					message = "An unexpected error occurred while cancelling the order."
				});
			}
		}

		[HttpPost("finish/{orderId}")]
		[AutoValidateAntiforgeryToken]
		public async Task<IActionResult> FinishOrder(int? orderId)
		{
			try
			{
				(bool isFinished, string message) = await this._saleService
									.FinishOrderAsync(orderId);

				return Ok(new
				{
					result = isFinished,
					message = message
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Something went wrong while finishing the Order!");

				return BadRequest(new
				{
					result = false,
					message = "An unexpected error occurred while finishing the order."
				});
			}
		}

		[HttpGet("customer-orders/{customerId}")]
		public async Task<IActionResult> GetOrdersByCustomer(string? customerId)
		{
			try
			{
				var orders = await this._saleService
										.GetCustomerOrderHistoryAsync(customerId);

				if (orders == null)
				{
					return BadRequest();
				}

				return Ok(orders);
			}
			catch (Exception ex)
			{
				this._logger.LogError(ex, "Something went wrong while getting the customer order history!");

				return BadRequest();
			}
		}

		[HttpGet("customer-insights")]
		public async Task<IActionResult> GetCustomerInsights(DateTime? fromDate, DateTime? toDate)
		{
			try
			{
				CustomerInsightsDto dto = await this._saleService
											.GetCustomersInsights(fromDate, toDate);

				return Ok(dto);
			}
			catch (Exception ex)
			{
				this._logger.LogError(ex, "Something went wrong while getting the customer insights!");
				 
				return BadRequest();
			}
		}
	}
}

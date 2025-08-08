using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Core.Admin.Interfaces;
using OnlineStore.Services.Core.DTO.Sales.Overview;

namespace OnlineStore.Web.Areas.Admin.Controllers.Api
{
	public class SaleApiController : BaseAdminApiController
	{
		private readonly ISaleService _saleService;
		private readonly ILogger<SaleApiController> _logger;

		public SaleApiController(ISaleService saleService, ILogger<SaleApiController> logger)
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
				_logger.LogError(ex, "Somethign went wrong while getting the Sale Overview!");

				return BadRequest();
			}
		}
	}
}

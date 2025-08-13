using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Core.Admin.Interfaces;
using OnlineStore.Web.ViewModels.Admin.Sale.OrderManagement;

namespace OnlineStore.Web.Areas.Admin.Controllers
{
	public class SaleController : BaseAdminController
	{
		private readonly IAdminSaleService _saleService;

		public SaleController(IAdminSaleService saleService)
		{
			this._saleService = saleService;
		}

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

		[HttpGet]
		public IActionResult OrderManagement()
		{
			AdminOrdersListViewModel model = new()
			{
				OrderStatuses = this._saleService.GetOrderStatusses()
			};

			return View(model);
		}

		[HttpGet]
		public IActionResult LocationSale()
		{
			return View();
		}

		[HttpGet]
		public IActionResult CustomerInsights()
		{
			return View();
		}

		[HttpGet]
		public IActionResult ProductAnalytics()
		{
			return View();
		}
	}
}

using OnlineStore.Services.Core.DTO.Sales.LocationSale;
using OnlineStore.Services.Core.DTO.Sales.OrderManagement;
using OnlineStore.Services.Core.DTO.Sales.Overview;
using OnlineStore.Services.Core.DTO.Sales.ProductAnalytics;
using OnlineStore.Web.ViewModels.Admin.Sale.LocationSales;
using OnlineStore.Web.ViewModels.Admin.Sale.ProductAnalytics;

namespace OnlineStore.Services.Core.Admin.Interfaces
{
	public interface ISaleService
	{

		Task<AdminSalesOverviewViewModel?> GetSaleOverviewAsync(DateTime? startDate, DateTime? endDate);

		Task<IEnumerable<OrderListItemViewModel>?> GetFilteredOrdersAsync(OrderFilterDto? dto);

		Task<OrderDetailsViewModel?> GetOrderDetailsAsync(int? orderId);

		Task<(bool isCancelled, string message)> CancelOrderAsync(int? orderId);

		Task<(bool isFinished, string message)> FinishOrderAsync(int? orderId);

		Task<ProductAnalyticsViewModel> GetProductAnalyticsAsync(ProductAnalyticsFilterDto? dto);

		Task<LocationSalesViewModel> GetSalesByLocationAsync(SalesByLocationDto dto);

		string[] GetOrderStatusses();
	}
}

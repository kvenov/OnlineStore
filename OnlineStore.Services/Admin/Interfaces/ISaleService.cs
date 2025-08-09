using OnlineStore.Services.Core.DTO.Sales.OrderManagement;
using OnlineStore.Services.Core.DTO.Sales.Overview;

namespace OnlineStore.Services.Core.Admin.Interfaces
{
	public interface ISaleService
	{

		Task<AdminSalesOverviewViewModel?> GetSaleOverviewAsync(DateTime? startDate, DateTime? endDate);

		Task<IEnumerable<OrderListItemViewModel>?> GetFilteredOrdersAsync(OrderFilterDto? dto);

		Task<OrderDetailsViewModel?> GetOrderDetailsAsync(int? orderId);

		Task<(bool isCancelled, string message)> CancelOrderAsync(int? orderId);

		Task<(bool isFinished, string message)> FinishOrderAsync(int? orderId);

		string[] GetOrderStatusses();
	}
}

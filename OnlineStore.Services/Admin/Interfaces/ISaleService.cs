using OnlineStore.Services.Core.DTO.Sales.Overview;

namespace OnlineStore.Services.Core.Admin.Interfaces
{
	public interface ISaleService
	{

		Task<AdminSalesOverviewViewModel?> GetSaleOverviewAsync(DateTime? startDate, DateTime? endDate);
	}
}

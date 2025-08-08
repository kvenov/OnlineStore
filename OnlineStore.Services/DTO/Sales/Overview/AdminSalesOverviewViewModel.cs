namespace OnlineStore.Services.Core.DTO.Sales.Overview
{
	public class AdminSalesOverviewViewModel
	{

		public decimal TotalRevenue { get; set; }
		public int TotalOrders { get; set; }
		public decimal AverageOrderValue { get; set; }

		public RevenueTrendData RevenueTrends { get; set; } = new();
		public PaymentMethodData PaymentMethods { get; set; } = new();
	}
}

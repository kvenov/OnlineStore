namespace OnlineStore.Web.ViewModels.Admin.Sale.ProductAnalytics
{
	public class ProductAnalyticsViewModel
	{
		public List<ProductSalesData> TopSellingProducts { get; set; } = new();

		public List<ProductSalesData> LeastSellingProducts { get; set; } = new();

		public List<SalesBySizeData> SalesBySize { get; set; } = new();
		public List<SalesByPriceRangeData> SalesByPriceRange { get; set; } = new();
	}
}

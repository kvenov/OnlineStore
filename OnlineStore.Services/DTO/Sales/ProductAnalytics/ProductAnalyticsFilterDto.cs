namespace OnlineStore.Services.Core.DTO.Sales.ProductAnalytics
{
	public class ProductAnalyticsFilterDto
	{
		public DateTime? FromDate { get; set; }
		public DateTime? ToDate { get; set; }
		public string? PriceRange { get; set; }
	}
}

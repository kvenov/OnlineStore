namespace OnlineStore.Services.Core.DTO.Sales.CustomerInsights
{
	public class CustomerInsightsDto
	{
		public decimal RepeatBuyerRate { get; set; }
		public decimal AverageLtv { get; set; }
		public int TotalCustomers { get; set; }
		public List<TopCustomerDto> TopCustomers { get; set; } = new();
	}
}

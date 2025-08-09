namespace OnlineStore.Services.Core.DTO.Sales.CustomerInsights
{
	public class TopCustomerDto
	{
		public string CustomerId { get; set; } = null!;
		public string Name { get; set; } = null!;
		public int OrdersCount { get; set; }
		public decimal TotalSpent { get; set; }
	}
}
namespace OnlineStore.Services.Core.DTO.Sales.OrderManagement
{
	public class OrderListItemViewModel
	{

		public int Id { get; set; }
		public string OrderNumber { get; set; } = null!;
		public string CustomerName { get; set; } = null!;
		public string CustomerEmail { get; set; } = null!;
		public DateTime Date { get; set; }
		public string Status { get; set; } = null!;
		public decimal Total { get; set; }
	}
}

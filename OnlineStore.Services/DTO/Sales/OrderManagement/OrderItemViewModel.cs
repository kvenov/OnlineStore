namespace OnlineStore.Services.Core.DTO.Sales.OrderManagement
{
	public class OrderItemViewModel
	{

		public string ProductName { get; set; } = null!;
		public string ProductImageUrl { get; set; } = null!;
		public string ProductSize { get; set; } = null!;
		public int Quantity { get; set; }
		public decimal Price { get; set; }
	}
}
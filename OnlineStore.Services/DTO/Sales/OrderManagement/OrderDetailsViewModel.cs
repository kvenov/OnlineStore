namespace OnlineStore.Services.Core.DTO.Sales.OrderManagement
{
	public class OrderDetailsViewModel
	{

		public int Id { get; set; }
		public string OrderNumber { get; set; } = null!;
		public DateTime OrderDate { get; set; }
		public string CustomerName { get; set; } = null!;
		public string CustomerEmail { get; set; } = null!;
		public string Status { get; set; } = null!;
		public string PaymentMethod { get; set; } = null!;
		public string BillingAddress { get; set; } = null!;
		public string ShippingAddress { get; set; } = null!;
		public string ShippingOption { get; set; } = null!;
		public string EstimateDeliveryStart { get; set; } = null!;
		public string EstimateDeliveryEnd { get; set; } = null!;

		public List<OrderItemViewModel> Items { get; set; } = new();

		public decimal Subtotal { get; set; }
		public decimal ShippingCost { get; set; }
		public decimal Total { get; set; }

		public string[] AvailableStatuses { get; set; } = null!;
		public bool CanCancel { get; set; }
		public bool CanRefund { get; set; }
	}
}

namespace OnlineStore.Web.ViewModels.Email
{
	public class OrderPlacedEmailModel
	{
		public string OrderNumber { get; set; } = null!;
		public DateTime OrderDate { get; set; }

		public string RecipientName { get; set; } = "Customer";
		public string ShippingAddress { get; set; } = null!;
		public string ShippingOption { get; set; } = null!;

		public string EstimatedDeliveryStartFormatted { get; set; } = null!;
		public string EstimatedDeliveryEndFormatted { get; set; } = null!;

		public decimal TotalAmount { get; set; }
		public string PaymentMethodName { get; set; } = null!;
		public string? CardLast4 { get; set; }

		public List<OrderEmailItem> Items { get; set; } = new();
		public string OrderDetailsUrl { get; set; } = null!;
	}

	public class OrderEmailItem
	{
		public string Name { get; set; } = null!;
		public int Qty { get; set; }
		public decimal Price { get; set; }
		public string ProductSize { get; set; } = null!;
	}
}

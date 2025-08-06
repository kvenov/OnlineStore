namespace OnlineStore.Web.ViewModels.Order
{
	public class OrderConfirmationViewModel
	{

		public string OrderNumber { get; set; } = null!;
		public DateTime EstimatedDeliveryStart { get; set; }
		public DateTime EstimatedDeliveryEnd { get; set; }

		public string EstimatedDeliveryStartFormatted =>
				EstimatedDeliveryStart.ToString("MMM dd");

		public string EstimatedDeliveryEndFormatted =>
				EstimatedDeliveryEnd.ToString("MMM dd");

		public string ShippingOption { get; set; } = null!;
		public decimal ShippingPrice { get; set; }
		public decimal TotalAmount { get; set; }
		public List<OrderProductViewModel> Products { get; set; } = new();
		public string RecipientName { get; set; } = null!;
		public string UserEmail { get; set; } = null!;
		public string ShippingAddress { get; set; } = null!;
		public bool IsGuest { get; set; }
	}
}

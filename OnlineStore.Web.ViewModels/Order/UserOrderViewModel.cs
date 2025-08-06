namespace OnlineStore.Web.ViewModels.Order
{
	public class UserOrderViewModel
	{

		public string OrderNumber { get; set; } = null!;

		public DateTime OrderDate { get; set; }

		public DateTime EstimatedDeliveryStart { get; set; }

		public DateTime EstimatedDeliveryEnd { get; set; }

		public string EstimatedDeliveryStartFormatted =>
				EstimatedDeliveryStart.ToString("MMM dd");

		public string EstimatedDeliveryEndFormatted =>
				EstimatedDeliveryEnd.ToString("MMM dd");

		public decimal TotalAmount { get; set; }

		public string ShippingOption { get; set; } = null!;

		public string Status { get; set; } = null!;

		public bool IsCompleted { get; set; }

		public bool IsCancelled { get; set; }

		public List<UserOrderItemViewModel> Items { get; set; } = new();
	}
}

using OnlineStore.Data.Models.Enums;

namespace OnlineStore.Web.ViewModels.Order
{
	public class OrderDetailsViewModel
	{

		public string OrderNumber { get; set; } = null!;

		public DateTime OrderDate { get; set; }

		public string Status { get; set; } = null!;

		public string ShippingOption { get; set; } = null!;

		public DateTime EstimatedDeliveryStart { get; set; }
		public DateTime EstimatedDeliveryEnd { get; set; }

		public string EstimatedDeliveryStartFormatted =>
				EstimatedDeliveryStart.ToString("MMM dd");

		public string EstimatedDeliveryEndFormatted =>
				EstimatedDeliveryEnd.ToString("MMM dd");

		public decimal TotalAmount { get; set; }

		public decimal ShippingPrice { get; set; }

		public string PaymentMethodName { get; set; } = null!;

		public PaymentMethodCode PaymentMethodCode { get; set; }

		public bool ShowPaymentDetails => PaymentMethodCode == PaymentMethodCode.CreditCard;

		public OrderPaymentDetailsViewModel? PaymentDetails { get; set; }

		public AddressViewModel ShippingAddress { get; set; } = null!;

		public AddressViewModel? BillingAddress { get; set; }

		public List<OrderProductViewModel> Items { get; set; } = new();
	}
}

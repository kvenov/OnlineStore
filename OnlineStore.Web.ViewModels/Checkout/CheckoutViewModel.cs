using OnlineStore.Web.ViewModels.Checkout.Partials;

namespace OnlineStore.Web.ViewModels.Checkout
{
	public class CheckoutViewModel
	{
		public bool IsGuest { get; set; }
		public string? UserId { get; set; }
		public string? GuestId { get; set; }

		public GuestAddressViewModel GuestAddress { get; set; } = new();
		public MemberAddressViewModel MemberAddress { get; set; } = new();
		public ShippingOptionsViewModel Shipping { get; set; } = new();
		public PaymentMethodViewModel Payment { get; set; } = new();
		public OrderSummaryViewModel Summary { get; set; } = new();
	}
}

using OnlineStore.Web.ViewModels.Checkout.Partials;
using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Web.ViewModels.Checkout
{
	public class CheckoutViewModel
	{
		[Required]
		public bool IsGuest { get; set; }
		public string? UserId { get; set; }
		public string? GuestId { get; set; }

		public GuestAddressViewModel? GuestAddress { get; set; }
		public MemberAddressViewModel? MemberAddress { get; set; }

		[Required]
		public ShippingOptionsViewModel Shipping { get; set; } = new();

		[Required]
		public PaymentMethodViewModel Payment { get; set; } = new();

		[Required]
		public OrderSummaryViewModel Summary { get; set; } = new();
	}
}

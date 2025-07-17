namespace OnlineStore.Web.ViewModels.Checkout.Partials
{
	public class GuestAddressViewModel
	{

		public string FullName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
	
		public GuestShippingAddressViewModel ShippingAddress { get; set; } = new();
		public GuestBillingAddressViewModel BillingAddress { get; set; } = new();
	}
}

namespace OnlineStore.Web.ViewModels.Checkout.Partials
{
	public class MemberAddressItemViewModel
	{

		public int Id { get; set; }
		public string Street { get; set; } = string.Empty;
		public string City { get; set; } = string.Empty;
		public string Country { get; set; } = string.Empty;
		public string ZipCode { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		public bool IsShippingAddress { get; set; }
	}
}

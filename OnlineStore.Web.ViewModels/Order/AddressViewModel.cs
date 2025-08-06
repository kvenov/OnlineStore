namespace OnlineStore.Web.ViewModels.Order
{
	public class AddressViewModel
	{

		public string FullAddress => $"{Street}, {City}, {ZipCode}, {Country}";

		public string Street { get; set; } = null!;

		public string City { get; set; } = null!;

		public string Country { get; set; } = null!;

		public string ZipCode { get; set; } = null!;

		public string PhoneNumber { get; set; } = null!;
	}
}

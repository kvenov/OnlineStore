namespace OnlineStore.Web.ViewModels.Checkout.Partials
{
	public class MemberAddressViewModel
	{

		public int? SelectedShippingAddressId { get; set; }

		public int? SelectedBillingAddressId { get; set; }

		public List<MemberAddressItemViewModel> SavedAddresses { get; set; } = new();

		public MemberAddressItemViewModel? NewShippingAddress { get; set; }

		public MemberAddressItemViewModel? NewBillingAddress { get; set; }
	}
}

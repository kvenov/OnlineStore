namespace OnlineStore.Web.ViewModels.Checkout.Partials
{
	public class ShippingOptionsViewModel
	{

		public ShippingOptionItemViewModel SelectedShippingOption { get; set; } = new();

		public List<ShippingOptionItemViewModel> Options { get; set; } = new();
	}
}

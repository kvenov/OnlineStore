namespace OnlineStore.Web.ViewModels.Checkout.Partials
{
	public class ShippingOptionsViewModel
	{

		public int? SelectedShippingOptionId { get; set; }

		public List<ShippingOptionItemViewModel> Options { get; set; } = new();
	}
}

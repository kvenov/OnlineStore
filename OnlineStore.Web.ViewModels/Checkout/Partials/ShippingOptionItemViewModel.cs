namespace OnlineStore.Web.ViewModels.Checkout.Partials
{
	public class ShippingOptionItemViewModel
	{

		public string Name { get; set; } = null!;

		public string Description { get; set; } = null!;

		public string DateRange { get; set; } = null!;

		public decimal Price { get; set; }
	}
}

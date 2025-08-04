namespace OnlineStore.Web.ViewModels.Checkout.Partials
{
	public class ShippingOptionItemViewModel
	{

		public string Name { get; set; } = null!;

		public string? Description { get; set; }

		public string DateRange { get; set; } = null!;

		public DateTime EstimatedDeliveryStart { get; set; }

		public DateTime EstimatedDeliveryEnd { get; set; }

		public decimal Price { get; set; }
	}
}

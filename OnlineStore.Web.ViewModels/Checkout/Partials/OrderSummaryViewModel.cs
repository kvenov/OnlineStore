namespace OnlineStore.Web.ViewModels.Checkout.Partials
{
	public class OrderSummaryViewModel
	{

		public List<OrderProductViewModel> Products { get; set; } = new();

		public decimal Subtotal { get; set; }

		public decimal DeliveryCost { get; set; }

		public decimal Total => Subtotal + DeliveryCost;
	}
}

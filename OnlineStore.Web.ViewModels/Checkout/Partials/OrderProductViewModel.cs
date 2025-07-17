namespace OnlineStore.Web.ViewModels.Checkout.Partials
{
	public class OrderProductViewModel
	{

		public int ProductId { get; set; }

		public string Name { get; set; } = string.Empty;

		public string ImageUrl { get; set; } = string.Empty;

		public int Quantity { get; set; }

		public string Size { get; set; } = string.Empty;

		public decimal UnitPrice { get; set; }

		public decimal TotalPrice => UnitPrice * Quantity;
	}
}

namespace OnlineStore.Web.ViewModels.Order
{
	public class UserOrderItemViewModel
	{

		public string Name { get; set; } = null!;

		public int Quantity { get; set; }

		public decimal Price { get; set; }

		public string ProductSize { get; set; } = null!;

		public string ImageUrl { get; set; } = null!;
	}
}

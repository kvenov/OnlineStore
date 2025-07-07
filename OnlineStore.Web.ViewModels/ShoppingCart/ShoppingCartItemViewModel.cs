namespace OnlineStore.Web.ViewModels.ShoppingCart
{
	public class ShoppingCartItemViewModel
	{

		public int Id { get; set; }
		public int ProductId { get; set; }
		public string ProductName { get; set; } = null!;
		public string ProductImageUrl { get; set; } = null!;
		public decimal UnitPrice { get; set; }
		public int Quantity { get; set; }
		public decimal TotalPrice => UnitPrice * Quantity;
	}
}

namespace OnlineStore.Web.ViewModels.ShoppingCart
{
	public class ShoppingCartSummaryViewModel
	{
		public decimal ItemTotalPrice { get; set; }
		public decimal SubTotal { get; set; }
		public decimal Shipping { get; set; }
		public decimal Total => SubTotal + Shipping;
	}
}

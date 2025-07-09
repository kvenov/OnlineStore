namespace OnlineStore.Web.ViewModels.ShoppingCart
{
	public class ShoppingCartSummaryViewModel
	{
		public decimal ItemTotalPrice { get; set; }
		public decimal SubTotal { get; set; }
		public decimal Shipping => SubTotal > 0 && SubTotal < 400 ? 10 : 0;
		public decimal Total => SubTotal + Shipping;
	}
}

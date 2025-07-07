namespace OnlineStore.Web.ViewModels.ShoppingCart
{
	public class ShoppingCartViewModel
	{

		public List<ShoppingCartItemViewModel> Items { get; set; } = new List<ShoppingCartItemViewModel>();
		public decimal SubTotal => Items.Sum(i => i.TotalPrice);
		public decimal Shipping => SubTotal > 0 && SubTotal < 100 ? 10 : 0;
		public decimal Total => SubTotal + Shipping;
	}
}

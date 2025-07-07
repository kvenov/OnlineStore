using OnlineStore.Web.ViewModels.ShoppingCart;

namespace OnlineStore.Web.ViewModels.Layout
{
	public class CartInfoViewModel
	{
		public List<ShoppingCartItemViewModel> Items { get; set; } = new List<ShoppingCartItemViewModel>();
		public decimal Total { get; set; }
	}
}

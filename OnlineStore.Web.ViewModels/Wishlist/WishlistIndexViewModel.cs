using OnlineStore.Web.ViewModels.Wishlist.Partial;

namespace OnlineStore.Web.ViewModels.Wishlist
{
	public class WishlistIndexViewModel
	{
		public int Id { get; set; }

		public string UserId { get; set; } = null!;
		public IEnumerable<WishlistItemPartialViewModel> Items { get; set; } =
					new List<WishlistItemPartialViewModel>();
	}
}

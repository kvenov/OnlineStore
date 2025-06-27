namespace OnlineStore.Web.ViewModels.Wishlist.Partial
{
	public class WishlistItemPartialViewModel
	{
		public int Id { get; set; }

		public string ProductName { get; set; } = null!;

		public string ProductCategory { get; set; } = null!;

		public string ImageUrl { get; set; } = null!;

		public decimal Price { get; set; }

		public string? Notes { get; set; }


	}
}

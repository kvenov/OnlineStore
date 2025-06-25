namespace OnlineStore.Web.ViewModels.Product.Partial
{
	public class ProductReviewPartialViewModel
	{
		public int Id { get; set; }

		public string PublisherId { get; set; } = null!;

		public string? Publisher { get; set; }

		public string Content { get; set; } = null!;
	}
}

namespace OnlineStore.Web.ViewModels.Home.Partial
{
	public class ProductPromotionViewModel
	{
		public int ProductId { get; set; }

		public string ImageUrl { get; set; } = null!;

		public string ProductName { get; set; } = null!;

		public string? PromotionTagline { get; set; }

	}
}

namespace OnlineStore.Web.ViewModels.Admin.Product
{
	public class AllProductsViewModel
	{

		public string Id { get; set; } = null!;

		public string Name { get; set; } = null!;

		public string Description { get; set; } = null!;

		public string Price { get; set; } = null!;

		public string? DiscountPrice { get; set; } = null!;

		public string Category { get; set; } = null!;

		public string? Brand { get; set; } = null!;

		public string ImageUrl { get; set; } = null!;

	}
}

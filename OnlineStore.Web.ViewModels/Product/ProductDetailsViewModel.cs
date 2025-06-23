using OnlineStore.Web.ViewModels.Product.Partial;

namespace OnlineStore.Web.ViewModels.Product
{
	public class ProductDetailsViewModel
	{

		public string Name { get; set; } = null!;
		public string Description { get; set; } = null!;
		public string Price { get; set; } = null!;
		public string? DiscountPrice { get; set; }
		public string ImageUrl { get; set; } = null!;
		public string AverageRating { get; set; } = null!;
		public string Category { get; set; } = null!;
		public string? Brand { get; set; }

		public IEnumerable<string> AvailableSizes = new List<string>();
		public ProductDetailsPartialViewModel Details { get; set; } = 
					new ProductDetailsPartialViewModel();
		public IEnumerable<ProductReviewPartialViewModel> Reviews { get; set; } = 
					new List<ProductReviewPartialViewModel>();
	}
}

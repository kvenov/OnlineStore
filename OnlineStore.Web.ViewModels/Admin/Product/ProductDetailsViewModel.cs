namespace OnlineStore.Web.ViewModels.Admin.Product
{
	public class ProductDetailsViewModel
	{

		public int Id { get; set; }

		public string Name { get; set; } = null!;
		public string Description { get; set; } = null!;
		public string ImageUrl { get; set; } = null!;

		public decimal Price { get; set; }
		public decimal? DiscountPrice { get; set; }

		public bool IsActive { get; set; }
		public int StockQuantity { get; set; }

		public double AverageRating { get; set; }
		public int TotalRatings { get; set; }

		public string Category { get; set; } = null!;
		public string? Brand { get; set; }

		// ProductDetails
		public string Material { get; set; } = null!;
		public string Color { get; set; } = null!;
		public string Gender { get; set; } = null!;
		public string SizeGuideUrl { get; set; } = null!;
		public string CountryOfOrigin { get; set; } = null!;
		public string CareInstructions { get; set; } = null!;
		public decimal Weight { get; set; }
		public string Fit { get; set; } = null!;
		public string Style { get; set; } = null!;
	}
}

namespace OnlineStore.Web.ViewModels.Product.Partial
{
	public class ProductDetailsPartialViewModel
	{
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

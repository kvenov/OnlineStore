using Microsoft.EntityFrameworkCore;

namespace OnlineStore.Data.Models
{

	[Comment("Details for a certain product")]
	public class ProductDetails
	{

		[Comment("Product details identifier")]
		public int Id { get; set; }

		[Comment("Product details material")]
		public string Material { get; set; } = null!;

		[Comment("Product details color")]
		public string Color { get; set; } = null!;

		[Comment("Product details gender")]
		public string Gender { get; set; } = null!;

		[Comment("Product details size guide url")]
		public string SizeGuideUrl { get; set; } = null!;

		[Comment("Product details country of origin")]
		public string CountryOfOrigin { get; set; } = null!;

		[Comment("Product details care instructions")]
		public string CareInstructions { get; set; } = null!;

		[Comment("Product details weight")]
		public decimal Weight { get; set; }

		[Comment("Product details fit")]
		public string Fit { get; set; } = null!;

		[Comment("Product details style")]
		public string Style { get; set; } = null!;


		public int ProductId { get; set; }

		[Comment("Product")]
		public virtual Product Product { get; set; } = null!;
	}
}

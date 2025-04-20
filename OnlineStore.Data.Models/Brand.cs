using Microsoft.EntityFrameworkCore;

namespace OnlineStore.Data.Models
{

	[Comment("Brands in the store")]
	public class Brand
	{

		[Comment("Brand identifier")]
		public int Id { get; set; }

		[Comment("Brand name")]
		public string Name { get; set; } = null!;

		[Comment("Brand logo url")]
		public string LogoUrl { get; set; } = null!;

		[Comment("Brand description")]
		public string? Description { get; set; }

		[Comment("Brand website url")]
		public string WebsiteUrl { get; set; } = null!;

		[Comment("Brand short info")]
		public bool IsActive { get; set; }

		[Comment("Products from that brand")]
		public virtual ICollection<Product> Products { get; set; } =
					new HashSet<Product>();
	}
}
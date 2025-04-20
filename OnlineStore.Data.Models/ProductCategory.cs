using Microsoft.EntityFrameworkCore;

namespace OnlineStore.Data.Models
{

	[Comment("Product categories in the store")]
	public class ProductCategory
	{

		[Comment("Product category identifier")]
		public int Id { get; set; }

		[Comment("Product category name")]
		public string Name { get; set; } = null!;

		[Comment("Product category description")]
		public string? Description { get; set; }


		[Comment("Products from that category")]
		public virtual ICollection<Product> Products { get; set; } = 
					new HashSet<Product>();
	}
}
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models.Interfaces;

namespace OnlineStore.Data.Models
{

	[Comment("Product categories in the store")]
	public class ProductCategory : ISoftDeletable
	{

		[Comment("Product category identifier")]
		public int Id { get; set; }

		[Comment("Product category name")]
		public string Name { get; set; } = null!;

		[Comment("Product category description")]
		public string? Description { get; set; }


		public int? ParentCategoryId { get; set; }

		[Comment("Parent category if this is a subcategory")]
		public virtual ProductCategory? ParentCategory { get; set; }

		[Comment("The sub categories of the category")]
		public virtual ICollection<ProductCategory> Subcategories { get; set; } = new HashSet<ProductCategory>();

		[Comment("Products from that category")]
		public virtual ICollection<Product> Products { get; set; } = 
					new HashSet<Product>();

		public bool IsDeleted { get; set; }
	}
}
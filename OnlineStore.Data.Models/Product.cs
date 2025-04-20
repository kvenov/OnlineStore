using Microsoft.EntityFrameworkCore;

namespace OnlineStore.Data.Models
{

	[Comment("Products in the store")]
	public class Product
	{

		[Comment("Product identifier")]
		public int Id { get; set; }

		[Comment("Product name")]
		public string Name { get; set; } = null!;

		[Comment("Product description")]
		public string Description { get; set; } = null!;

		[Comment("Product price")]
		public decimal Price { get; set; }

		[Comment("Product discount price")]
		public decimal? DiscountPrice { get; set; }

		[Comment("Product quantity")]
		public int StockQuantity { get; set; }

		[Comment("Product short info")]
		public bool IsActive { get; set; }

		[Comment("Product creation data")]
		public DateTime CreatedAt { get; set; }

		[Comment("Product image url")]
		public string ImageUrl { get; set; } = null!;



		public int CategoryId { get; set; }

		[Comment("Product category")]
		public ProductCategory Category { get; set; } = null!;

		public int BrandId { get; set; }

		[Comment("Product brand")]
		public virtual Brand Brand { get; set; } = null!;


		public virtual ICollection<OrderItem> OrderItems { get; set; } =
					new HashSet<OrderItem>();

		public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } =
					new HashSet<ShoppingCartItem>();
	}
}

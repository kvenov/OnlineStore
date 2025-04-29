using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models.Interfaces;

namespace OnlineStore.Data.Models
{

	[Comment("Products in the store")]
	public class Product : ISoftDeletable
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

		[Comment("Product avarage rating")]
		public double AverageRating { get; set; }

		[Comment("Product total ratings")]
		public int TotalRatings { get; set; }



		public int CategoryId { get; set; }

		[Comment("Product category")]
		public virtual ProductCategory Category { get; set; } = null!;

		public int? BrandId { get; set; }

		[Comment("Product brand")]
		public virtual Brand? Brand { get; set; }


		public int ProductDetailsId { get; set; }

		[Comment("Product details")]
		public virtual ProductDetails ProductDetails { get; set; } = null!;

		[Comment("Orders items that contained the current Product")]
		public virtual ICollection<OrderItem> OrderItems { get; set; } =
					new HashSet<OrderItem>();

		[Comment("Shopping cart items that contained the current Product")]
		public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } =
					new HashSet<ShoppingCartItem>();

		[Comment("Wishlist items that contained the current Product")]
		public virtual ICollection<WishlistItem> WishlistItems { get; set; } =
					new HashSet<WishlistItem>();

		[Comment("Product ratings")]
		public virtual ICollection<ProductRating> ProductRatings { get; set; } =
					new HashSet<ProductRating>();

		public bool IsDeleted { get; set; }
	}
}

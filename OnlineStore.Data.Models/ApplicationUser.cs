using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models.Interfaces;

namespace OnlineStore.Data.Models
{

	[Comment("Users in the store")]
	public class ApplicationUser : IdentityUser, ISoftDeletable
	{

		[Comment("User Fullname")]
		public string? FullName { get; set; }

		[Comment("Registration date of the user")]
		public DateTime CreatedDate { get; set; }

		[Comment("Shopping cart of the user")]
		public virtual ShoppingCart ShoppingCart { get; set; } = null!;

		[Comment("Wishlist of the user")]
		public virtual Wishlist Wishlist { get; set; } = null!;

		[Comment("User Addresses")]
		public virtual ICollection<Address> Addresses { get; set; } =
					new HashSet<Address>();

		[Comment("The User's articles")]
		public virtual ICollection<Article> Articles { get; set; } =
					new HashSet<Article>();

		[Comment("The User's orders")]
		public virtual ICollection<Order> Orders { get; set; } =
					new HashSet<Order>();

		[Comment("The User's recently viewed products")]
		public virtual ICollection<RecentlyViewedProduct> RecentlyViewedProducts { get; set; } =
					new HashSet<RecentlyViewedProduct>();

		[Comment("The User's products reviews")]
		public virtual ICollection<ProductRating> ProductRatings { get; set; } =
					new HashSet<ProductRating>();

		[Comment("The User's checkouts")]
		public virtual ICollection<Checkout> Checkouts { get; set; } =
					new HashSet<Checkout>();

		[Comment("The User's product reviews")]
		public virtual ICollection<ProductReview> ProductReviews { get; set; } =
					new HashSet<ProductReview>();

		public bool IsDeleted { get; set; }
	}
}

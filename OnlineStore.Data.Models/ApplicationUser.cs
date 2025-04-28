using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineStore.Data.Models
{

	[Comment("Users in the store")]
	public class ApplicationUser : IdentityUser
	{

		[Comment("User Fullname")]
		public string? FullName { get; set; }

		[Comment("Registration date of the user")]
		public DateTime CreatedDate { get; set; }

		[Comment("User Address")]
		public string? DefaultAddress { get; set; }


		//For now, we will keep this property as not-included, and latter we will think to do something with it
		[NotMapped]
		[Comment("User Loyalty points")]
		public int? LoyaltyPoints { get; set; }


		public int ShoppingCartId { get; set; }

		[Comment("Shopping cart of the user")]
		public virtual ShoppingCart ShoppingCart { get; set; } = null!;

		public int WishlistId { get; set; }

		[Comment("Wishlist of the user")]
		public virtual Wishlist Wishlist { get; set; } = null!;

		[Comment("The User's articles")]
		public virtual ICollection<Article> Articles { get; set; } =
					new HashSet<Article>();

		[Comment("The User's orders")]
		public virtual ICollection<Order> Orders { get; set; } =
					new HashSet<Order>();

		[Comment("The User's products reviews")]
		public virtual ICollection<ProductRating> ProductRatings { get; set; } =
					new HashSet<ProductRating>();
	}
}

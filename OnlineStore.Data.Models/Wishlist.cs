using Microsoft.EntityFrameworkCore;

namespace OnlineStore.Data.Models
{

	[Comment("The Wishlist in the store")]
	public class Wishlist
	{

		[Comment("The Wishlist Id")]
		public int Id { get; set; }

		public string UserId { get; set; } = null!;

		[Comment("The User that owns the Wishlist")]
		public virtual ApplicationUser User { get; set; } = null!;

		[Comment("The Wishlist items")]
		public virtual ICollection<WishlistItem> WishlistItems { get; set; } = 
					new HashSet<WishlistItem>();
	}
}

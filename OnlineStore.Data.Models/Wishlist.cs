using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models.Interfaces;

namespace OnlineStore.Data.Models
{

	[Comment("The Wishlist in the store")]
	public class Wishlist : ISoftDeletable
	{

		[Comment("The Wishlist Id")]
		public int Id { get; set; }

		public string UserId { get; set; } = null!;

		[Comment("The User that owns the Wishlist")]
		public virtual ApplicationUser User { get; set; } = null!;

		[Comment("The Wishlist items")]
		public virtual ICollection<WishlistItem> WishlistItems { get; set; } = 
					new HashSet<WishlistItem>();

		public bool IsDeleted { get; set; }
	}
}

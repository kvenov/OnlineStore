using Microsoft.EntityFrameworkCore;

namespace OnlineStore.Data.Models
{

	[Comment("The wishlists items in the store")]
	public class WishlistItem
	{

		[Comment("The WishlistItem Id")]
		public int Id { get; set; }

		[Comment("The WishlistItem created date")]
		public DateTime AddedAt { get; set; }

		[Comment("The WishlistItem notes")]
		public string? Notes { get; set; }

		[Comment("The WishlistItem quantity")]
		public int Quantity { get; set; }

		public int WishlistId { get; set; }

		[Comment("The Wishlist that owns the WishlistItem")]
		public virtual Wishlist Wishlist { get; set; } = null!;

		public int ProductId { get; set; }

		[Comment("The Product that will be contained in the Wishlist")]
		public virtual Product Product { get; set; } = null!;
	}
}
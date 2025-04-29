using Microsoft.EntityFrameworkCore;

namespace OnlineStore.Data.Models
{

	[Comment("The Recently(lastly) viewed products in the store")]
	public class RecentlyViewedProduct
	{

		[Comment("The RecentlyViewed Product identifier")]
		public int Id { get; set; }

		public int ProductId { get; set; }

		[Comment("The RecentlyViewed Product")]
		public virtual Product Product { get; set; } = null!;

		public string UserId { get; set; } = null!;

		[Comment("The RecentlyViewed Product User(owner)")]
		public virtual ApplicationUser User { get; set; } = null!;

		[Comment("The RecentlyViewed Product creation date")]
		public DateTime ViewedAt { get; set; }
	}
}

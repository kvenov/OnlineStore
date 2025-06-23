using Microsoft.EntityFrameworkCore;

namespace OnlineStore.Data.Models
{

	[Comment("The Product Reviews in the store")]
	public class ProductReview
	{

		[Comment("Product review identifier")]
		public int Id { get; set; }

		public int ProductId { get; set; }

		[Comment("Product that the review is for")]
		public virtual Product Product { get; set; } = null!;

		public string UserId { get; set; } = null!;

		[Comment("User that wrote the review")]
		public virtual ApplicationUser User { get; set; } = null!;

		[Comment("Product review content")]
		public string Content { get; set; } = null!;
	}
}

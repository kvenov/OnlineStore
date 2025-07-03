using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models.Interfaces;

namespace OnlineStore.Data.Models
{

	[Comment("The promotions in the store")]
	public class ProductPromotion : ISoftDeletable
	{

		[Comment("The promotion identifier")]
		public int Id { get; set; }

		public int ProductId { get; set; }

		[Comment("The promotion product")]
		public virtual Product Product { get; set; } = null!;

		[Comment("The promotion price")]
		public decimal PromotionPrice { get; set; }

		[Comment("Marketing label or title for the promotion")]
		public string Label { get; set; } = null!;

		[Comment("The promotion start date")]
		public DateTime StartDate { get; set; }

		[Comment("The promotion date of expire")]
		public DateTime ExpDate { get; set; }

		[Comment("Whether the promotion is currently active")]
		public bool IsDeleted { get; set; }
	}
}

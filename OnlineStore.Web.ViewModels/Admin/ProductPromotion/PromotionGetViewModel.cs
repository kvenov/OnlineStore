namespace OnlineStore.Web.ViewModels.Admin.ProductPromotion
{
	public class PromotionGetViewModel
	{
		public int Id { get; set; }

		public int ProductId { get; set; }

		public string PromotionPrice { get; set; } = null!;

		public string Label { get; set; } = null!;

		public string StartDate { get; set; } = null!;

		public string ExpDate { get; set; } = null!;

		public bool IsDeleted { get; set; }
	}
}

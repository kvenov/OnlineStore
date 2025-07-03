namespace OnlineStore.Web.ViewModels.Admin.ProductPromotion
{
	public class PromotionIndexViewModel
	{

		public int Id { get; set; }

		public decimal PromotionPrice { get; set; }

		public string ProductName { get; set; } = null!;

		public string Label { get; set; } = null!;

		public DateTime StartDate { get; set; }

		public DateTime ExpDate { get; set; }

		public bool IsDeleted { get; set; }
	}
}

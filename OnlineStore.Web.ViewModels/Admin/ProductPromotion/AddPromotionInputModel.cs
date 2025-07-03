using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using static OnlineStore.Data.Common.Constants.EntityConstants.ProductPromotion;

namespace OnlineStore.Web.ViewModels.Admin.ProductPromotion
{
	public class AddPromotionInputModel
	{

		[Required]
		public int ProductId { get; set; }

		[Required]
		[DataType(DataType.Currency)]
		public string PromotionPrice { get; set; } = null!;

		[Required]
		[StringLength(LabelMaxLength, MinimumLength = LabelMinLength)]
		public string Label { get; set; } = null!;

		[Required]
		[DataType(DataType.DateTime)]
		public string StartDate { get; set; } = null!;

		[Required]
		[DataType(DataType.DateTime)]
		public string ExpDate { get; set; } = null!;

		[Required]
		public string IsDeleted { get; set; } = null!;
	}
}

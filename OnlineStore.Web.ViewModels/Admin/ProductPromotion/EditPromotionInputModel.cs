using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Web.ViewModels.Admin.ProductPromotion
{
	public class EditPromotionInputModel : AddPromotionInputModel
	{

		[Required]
		public int Id { get; set; }
	}
}

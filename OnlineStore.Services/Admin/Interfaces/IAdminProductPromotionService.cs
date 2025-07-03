using OnlineStore.Web.ViewModels.Admin.ProductPromotion;

namespace OnlineStore.Services.Core.Admin.Interfaces
{
	public interface IAdminProductPromotionService
	{

		Task<IEnumerable<PromotionIndexViewModel>> GetProductsPromotionsAsync();

		Task<bool> CreateProductPromotion(AddPromotionInputModel? model);
	}
}

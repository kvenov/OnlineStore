using OnlineStore.Web.ViewModels.Home.Partial;

namespace OnlineStore.Services.Core.Interfaces
{
	public interface IProductPromotionService
	{

		Task<IEnumerable<ProductPromotionViewModel>> GetProductsPromotionsAsync();
	}
}

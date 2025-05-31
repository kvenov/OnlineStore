using OnlineStore.Web.ViewModels;

namespace OnlineStore.Services.Core.Interfaces
{
	public interface IProductService
	{
		Task<IEnumerable<ProductViewModel>> GetAllProductsAsync();
		Task<ProductViewModel> GetProductByIdAsync(int id);

	}
}

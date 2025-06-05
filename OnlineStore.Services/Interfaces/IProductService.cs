using OnlineStore.Web.ViewModels.Product;

namespace OnlineStore.Services.Core.Interfaces
{
	public interface IProductService
	{
		Task<IEnumerable<AllProductListViewModel>> GetAllProductsAsync();
		Task<AllProductListViewModel> GetProductByIdAsync(int id);

	}
}

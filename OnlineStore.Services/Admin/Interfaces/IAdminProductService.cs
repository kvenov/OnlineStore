using OnlineStore.Web.ViewModels.Admin.Product;

namespace OnlineStore.Services.Core.Admin.Interfaces
{
	public interface IAdminProductService
	{
		Task<IEnumerable<AllProductsViewModel>> GetAllProductsAsync();

		Task AddProductAsync(AddProductViewModel model);
	}
}

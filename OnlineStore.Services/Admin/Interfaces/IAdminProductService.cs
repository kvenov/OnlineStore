using OnlineStore.Web.ViewModels.Admin.Product;

namespace OnlineStore.Services.Core.Admin.Interfaces
{
	public interface IAdminProductService
	{
		Task<IEnumerable<AllProductsViewModel>> GetAllProductsAsync();

		Task<ProductDetailsForDeleteViewModel?> GetProductDetailsForDeleteAsync(string id);

		Task<bool> AddProductAsync(AddProductInputModel model);

		Task<EditProductInputModel?> GetEditableProductByIdAsync(int? id);

		Task<bool> EditProductAsync(EditProductInputModel? model);

		Task<bool> SoftDeleteProductAsync(string? id);
	}
}

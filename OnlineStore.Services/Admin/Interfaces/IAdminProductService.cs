using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineStore.Data.Models;
using OnlineStore.Web.ViewModels.Admin.Product;
using OnlineStore.Web.ViewModels.Admin.ProductPromotion;

namespace OnlineStore.Services.Core.Admin.Interfaces
{
	public interface IAdminProductService
	{
		Task<IEnumerable<AllProductsViewModel>> GetAllProductsAsync();

		Task<ProductDetailsForDeleteViewModel?> GetProductDetailsForDeleteAsync(string id);

		Task<ProductDetailsViewModel?> GetProductDetailsByIdAsync(int? id);

		Task<bool> AddProductAsync(AddProductInputModel model);

		Task<EditProductInputModel?> GetEditableProductByIdAsync(int? id);

		Task<bool> EditProductAsync(EditProductInputModel? model);

		Task<bool> SoftDeleteProductAsync(string? id);

		Task<IEnumerable<PromotionProductViewModel>> GetProductsIdsAndNamesAsync();


		IEnumerable<SelectListItem> GetGendersForProductDetails();
	}
}

using OnlineStore.Web.ViewModels.Home.Partial;
using OnlineStore.Web.ViewModels.Product;
using OnlineStore.Services.Core.DTO.Product;

namespace OnlineStore.Services.Core.Interfaces
{
	public interface IProductService
	{
		Task<IEnumerable<AllProductListViewModel>> GetAllProductsAsync();

		Task<SearchProductListViewModel> GetAllProductsAsync(string? query);

		Task<AllProductListViewModel?> GetProductByIdAsync(int? id);

		Task<ProductDetailsViewModel?> GetProductDetailsByIdAsync(int? productId, string? userId);

		Task<bool> AddProductReviewAsync(int? productId, int? rating, string? content, string userId);

		Task<bool> EditProductReviewAsync(int? reviewId, int? rating, int? ratingId, string? content, string userId);

		Task<bool> RemoveProductReviewAsync(int? reviewId, int? ratingId, string userId);

		Task<IEnumerable<TrendingProductViewModel>> GetBestProductsAsync();

		Task<IEnumerable<AllProductListViewModel>> GetFilteredProductsAsync(string? gender, string? category, string? subCategory);

		Task<IEnumerable<GetSearchedProductsDto>> GetSearchedProductsAsync(string? query, int maxResults = 5);

		Task<IEnumerable<string>?> GetProductSizesAsync(int? productId);
	}
}

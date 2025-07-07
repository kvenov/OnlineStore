using OnlineStore.Data.Models;

namespace OnlineStore.Data.Repository.Interfaces
{
	public interface IProductRepository : IAsyncRepository<Product, int>, IRepository<Product, int>
	{

		IQueryable<ProductReview> GetAllReviewsAttached();

		IQueryable<ProductRating> GetAllRatingsAttached();

		Task AddProductRatingAsync(ProductRating productRating);

		Task AddProductReviewAsync(ProductReview productReview);

		Task<ProductRating?> GetProductRatingByIdAsync(int? id);

		Task<ProductReview?> GetProductReviewByIdAsync(int? id);

		Task<bool> DeleteReviewAsync(ProductReview? review);

		Task<bool> DeleteRatingAsync(ProductRating? rating);

	}
}

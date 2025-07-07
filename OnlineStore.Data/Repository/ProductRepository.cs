using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;

namespace OnlineStore.Data.Repository
{
	public class ProductRepository : BaseRepository<Product, int>, IProductRepository
	{
		public ProductRepository(ApplicationDbContext dbContext) : 
					base(dbContext)
		{
		}

		public async Task AddProductRatingAsync(ProductRating productRating)
		{
			await this.DbContext.ProductsRatings.AddAsync(productRating);
		}

		public async Task AddProductReviewAsync(ProductReview productReview)
		{
			await this.DbContext.ProductReviews.AddAsync(productReview);
		}

		public async Task<bool> DeleteRatingAsync(ProductRating? rating)
		{
			bool result = false;
			if (rating != null)
			{
				rating.IsDeleted = true;

				int affectedRows = await this.DbContext.SaveChangesAsync();
				result = affectedRows > 0;
			}

			return result;
		}

		public async Task<bool> DeleteReviewAsync(ProductReview? review)
		{
			bool result = false;
			if (review != null)
			{
				review.IsDeleted = true;

				int affectedRows = await this.DbContext.SaveChangesAsync();
				result = affectedRows > 0;
			}

			return result;
		}

		public IQueryable<ProductRating> GetAllRatingsAttached()
		{
			return this.DbContext.ProductsRatings
						.AsQueryable();
		}

		public IQueryable<ProductReview> GetAllReviewsAttached()
		{
			return this.DbContext.ProductReviews
						.AsQueryable();
		}

		public async Task<ProductRating?> GetProductRatingByIdAsync(int? id)
		{
			ProductRating? rating = null;

			if (id != null)
			{
				rating = await this.DbContext.ProductsRatings
							.FindAsync(id);
			}

			return rating;
		}

		public async Task<ProductReview?> GetProductReviewByIdAsync(int? id)
		{
			ProductReview? review = null;

			if (id != null)
			{
				review = await this.DbContext.ProductReviews
							.FindAsync(id);
			}

			return review;
		}
	}
}

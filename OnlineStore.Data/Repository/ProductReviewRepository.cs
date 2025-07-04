using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;

namespace OnlineStore.Data.Repository
{
	public class ProductReviewRepository : BaseRepository<ProductReview, int>, IProductReviewRepository
	{
		public ProductReviewRepository(ApplicationDbContext dbContext) 
					: base(dbContext)
		{
		}
	}
}

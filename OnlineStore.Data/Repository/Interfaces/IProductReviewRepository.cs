using OnlineStore.Data.Models;

namespace OnlineStore.Data.Repository.Interfaces
{
	public interface IProductReviewRepository : IRepository<ProductReview, int>, IAsyncRepository<ProductReview, int>
	{
	}
}

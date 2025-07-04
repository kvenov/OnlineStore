using OnlineStore.Data.Models;

namespace OnlineStore.Data.Repository.Interfaces
{ 
	public interface IProductRatingRepository : IRepository<ProductRating, int>, IAsyncRepository<ProductRating, int>
	{
	}
}

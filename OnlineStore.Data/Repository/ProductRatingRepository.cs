using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;

namespace OnlineStore.Data.Repository
{
	public class ProductRatingRepository : BaseRepository<ProductRating, int>, IProductRatingRepository
	{
		public ProductRatingRepository(ApplicationDbContext dbContext) : 
					base(dbContext)
		{
		}
	}
}

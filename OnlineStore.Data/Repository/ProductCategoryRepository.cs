using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;

namespace OnlineStore.Data.Repository
{
	public class ProductCategoryRepository : BaseRepository<ProductCategory, int>, IProductCategoryRepository
	{
		public ProductCategoryRepository(ApplicationDbContext dbContext) 
				: base(dbContext)
		{
		}
	}
}

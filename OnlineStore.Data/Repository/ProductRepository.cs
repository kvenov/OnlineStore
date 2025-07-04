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
	}
}

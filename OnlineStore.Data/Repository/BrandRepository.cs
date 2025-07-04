using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;

namespace OnlineStore.Data.Repository
{
	public class BrandRepository : BaseRepository<Brand, int>, IBrandRepository
	{
		public BrandRepository(ApplicationDbContext dbContext) 
				: base(dbContext)
		{
		}
	}
}

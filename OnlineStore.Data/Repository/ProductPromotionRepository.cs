using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;

namespace OnlineStore.Data.Repository
{
	public class ProductPromotionRepository : BaseRepository<ProductPromotion, int>, IProductPromotionRepository
	{
		public ProductPromotionRepository(ApplicationDbContext dbContext) : 
				base(dbContext)
		{
		}
	}
}

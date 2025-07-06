using OnlineStore.Data.Models;

namespace OnlineStore.Data.Repository.Interfaces
{
	public interface IProductPromotionRepository : IRepository<ProductPromotion, int>, IAsyncRepository<ProductPromotion, int>
	{
	}
}

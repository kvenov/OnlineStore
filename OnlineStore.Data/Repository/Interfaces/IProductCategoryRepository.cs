using OnlineStore.Data.Models;

namespace OnlineStore.Data.Repository.Interfaces
{
	public interface IProductCategoryRepository : IAsyncRepository<ProductCategory, int>, IRepository<ProductCategory, int>
	{
	}
}

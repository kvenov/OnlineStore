using OnlineStore.Data.Models;

namespace OnlineStore.Data.Repository.Interfaces
{
	public interface IProductRepository : IAsyncRepository<Product, int>, IRepository<Product, int>
	{
	}
}

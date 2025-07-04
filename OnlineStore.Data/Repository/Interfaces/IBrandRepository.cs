using OnlineStore.Data.Models;

namespace OnlineStore.Data.Repository.Interfaces
{
	public interface IBrandRepository : IRepository<Brand, int>, IAsyncRepository<Brand, int>
	{
	}
}

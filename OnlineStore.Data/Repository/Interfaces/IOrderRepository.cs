using OnlineStore.Data.Models;

namespace OnlineStore.Data.Repository.Interfaces
{
	public interface IOrderRepository : IRepository<Order, int>, IAsyncRepository<Order, int>
	{
	}
}

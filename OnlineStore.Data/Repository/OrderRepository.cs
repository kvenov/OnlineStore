using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;

namespace OnlineStore.Data.Repository
{
	public class OrderRepository : BaseRepository<Order, int>, IOrderRepository
	{
		public OrderRepository(ApplicationDbContext dbContext) 
					: base(dbContext)
		{
		}
	}
}

using OnlineStore.Data.Models;

namespace OnlineStore.Data.Repository.Interfaces
{
	public interface ICheckoutRepository : IRepository<Checkout, int>, IAsyncRepository<Checkout, int>
	{
	}
}

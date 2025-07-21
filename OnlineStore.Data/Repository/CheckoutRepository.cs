using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;

namespace OnlineStore.Data.Repository
{
	public class CheckoutRepository : BaseRepository<Checkout, int>, ICheckoutRepository
	{
		public CheckoutRepository(ApplicationDbContext dbContext) 
					: base(dbContext)
		{
		}
	}
}

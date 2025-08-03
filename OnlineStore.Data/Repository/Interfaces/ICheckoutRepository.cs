using OnlineStore.Data.Models;

namespace OnlineStore.Data.Repository.Interfaces
{
	public interface ICheckoutRepository : IRepository<Checkout, int>, IAsyncRepository<Checkout, int>
	{

		Task RefreshCheckoutTotalsAsync(string? userId, int checkoutId);

		Task RefreshCheckoutStartingDateAsync(int checkoutId);

		Task<decimal> GetCheckoutDeliveryCostAsync(string? userId, decimal? subTotal);
	}
}

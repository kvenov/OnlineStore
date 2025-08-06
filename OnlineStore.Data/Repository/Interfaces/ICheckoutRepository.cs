using OnlineStore.Data.Models;

namespace OnlineStore.Data.Repository.Interfaces
{
	public interface ICheckoutRepository : IRepository<Checkout, int>, IAsyncRepository<Checkout, int>
	{

		Task RefreshCheckoutTotalsAsync(string? userId, int checkoutId);

		Task RefreshCheckoutStartingDateAsync(int checkoutId);

		Task UpdateCheckoutFromUserDefaultsAsync(int? checkoutId, ApplicationUser? user);

		Task UpdateCheckoutFromLastOrderAsync(int? checkoutId, string? userId);

		Task<decimal> GetCheckoutDeliveryCostAsync(string? userId, decimal? subTotal);

		Task SetCheckoutDefaultPaymentMethodAsync(Checkout? checkout);

		void SetCheckoutDefaultShippingOption(Checkout? checkout, ApplicationUser? user);
	}
}

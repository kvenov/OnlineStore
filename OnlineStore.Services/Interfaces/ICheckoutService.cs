using OnlineStore.Web.ViewModels.Checkout;

namespace OnlineStore.Services.Core.Interfaces
{
	public interface ICheckoutService
	{

		Task<CheckoutViewModel?> GetUserCheckoutAsync(string? userId);
	}
}

using OnlineStore.Data.Models;
using OnlineStore.Web.ViewModels.Checkout;

namespace OnlineStore.Services.Core.Interfaces
{
	public interface ICheckoutService
	{
		Task<Checkout?> InitializeCheckoutAsync(string? userId);

		CheckoutViewModel? GetUserCheckout(Checkout? checkout);
	}
}

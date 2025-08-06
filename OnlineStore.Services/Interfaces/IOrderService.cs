using OnlineStore.Data.Models;
using OnlineStore.Web.ViewModels.Order;

namespace OnlineStore.Services.Core.Interfaces
{
	public interface IOrderService
	{

		Task<int?> CreateOrderAsync(Checkout? checkout);

		Task<IEnumerable<UserOrderViewModel>?> GetUserOrdersAsync(string? userId);

		Task<OrderConfirmationViewModel?> GetOrderForConfirmationPageAsync(string? userId, int? orderId);
	}
}

using OnlineStore.Data.Models;

namespace OnlineStore.Services.Core.Email.Interfaces
{
	public interface IOrderEmailService
	{
		Task SendOrderPlacedAsync(Order order, CancellationToken ct = default);
	}
}

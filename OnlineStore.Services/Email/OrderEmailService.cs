using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Services.Core.Email.Interfaces;
using OnlineStore.Web.ViewModels.Email;

namespace OnlineStore.Services.Core.Email
{
	public class OrderEmailService : IOrderEmailService
	{
		private readonly IViewRenderService _viewRenderer;
		private readonly IRepository<OrderItem, int> _orderItemRepo;
		private readonly IEmailSender _sender;
		private readonly IConfiguration _config;

		public OrderEmailService(IViewRenderService viewRenderer, IEmailSender sender, 
								IConfiguration config, IRepository<OrderItem, int> orderItemRepo)
		{
			this._viewRenderer = viewRenderer;
			this._sender = sender;
			this._config = config;
			this._orderItemRepo = orderItemRepo;
		}

		public async Task SendOrderPlacedAsync(Order order, CancellationToken ct = default)
		{
			var toEmail = order.User?.Email ?? order.GuestEmail ?? throw new InvalidOperationException("No recipient email.");
			var toName = order.User?.UserName ?? order.GuestName ?? "Customer";

			var addr = $"{order.ShippingAddress.Street}, {order.ShippingAddress.City} {order.ShippingAddress.ZipCode}, {order.ShippingAddress.Country}";

			var orderItems = await _orderItemRepo
							.GetAllAttached()
							.Where(oi => oi.OrderId == order.Id)
							.ToListAsync(cancellationToken: ct);
			
			var items = orderItems.Select(i => new OrderEmailItem
			{
				Name = i.Product?.Name ?? $"Product #{i.ProductId}",
				Qty = i.Quantity,
				Price = i.Subtotal,
				ProductSize = i.ProductSize
			}).ToList();

			string? last4 = null;
			if (order.PaymentMethod?.Name?.Equals("Credit Card", StringComparison.OrdinalIgnoreCase) == true &&
				order.PaymentDetails?.CardNumber is string cn && cn.Length >= 4)
			{
				last4 = cn[^4..];
			}

			var baseUrl = _config["Email:BaseUrl"]?.TrimEnd('/') ?? "https://localhost:5001";
			var orderDetailsUrl = $"{baseUrl}/Order/Details/{order.OrderNumber}";

			var model = new OrderPlacedEmailModel
			{
				OrderNumber = order.OrderNumber,
				OrderDate = order.OrderDate,
				RecipientName = toName,
				ShippingAddress = addr,
				ShippingOption = order.ShippingOption,
				EstimatedDeliveryStartFormatted = order.EstimatedDeliveryStart.ToString("MMM dd"),
				EstimatedDeliveryEndFormatted = order.EstimatedDeliveryEnd.ToString("MMM dd"),
				TotalAmount = order.TotalAmount,
				PaymentMethodName = order.PaymentMethod?.Name ?? "—",
				CardLast4 = last4,
				Items = items,
				OrderDetailsUrl = orderDetailsUrl
			};

			var html = await _viewRenderer.RenderToStringAsync("/Views/Email/OrderPlaced.cshtml", model);
			var subject = $"Your order #{order.OrderNumber} at OnlineStore";

			await _sender.SendAsync(toEmail, toName, subject, html,
				textBody: $"Order #{order.OrderNumber} placed on {order.OrderDate:MMM dd, yyyy}. Total: {order.TotalAmount:C}.",
				ct: ct);
		}
	}
}

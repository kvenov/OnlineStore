using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models;
using OnlineStore.Data.Models.Enums;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Services.Core.Admin.Interfaces;
using OnlineStore.Services.Core.DTO.Sales.OrderManagement;
using OnlineStore.Services.Core.DTO.Sales.Overview;
using System.Text;

namespace OnlineStore.Services.Core.Admin
{
	public class SaleService : ISaleService
	{
		private readonly IOrderRepository _orderRepository;

		public SaleService(IOrderRepository orderRepository)
		{
			this._orderRepository = orderRepository;
		}

		public async Task<IEnumerable<OrderListItemViewModel>?> GetFilteredOrdersAsync(OrderFilterDto? dto)
		{
			IEnumerable<OrderListItemViewModel>? filteredOrders = null;

			if (dto != null)
			{
				var query = this._orderRepository
									.GetAllAttached()
									.AsNoTracking();

				if (!string.IsNullOrWhiteSpace(dto.OrderNumber))
				{
					query = query.Where(o => o.OrderNumber.ToLower() == dto.OrderNumber.ToLower());
				}

				if (!string.IsNullOrWhiteSpace(dto.Customer))
				{
					query = query.Where(o =>
						(o.User != null && o.User.UserName!.ToLower().Contains(dto.Customer.ToLower())) ||
						(o.GuestName != null && o.GuestName.ToLower().Contains(dto.Customer.ToLower())));
				}

				if (dto.DateFrom.HasValue)
				{
					query = query.Where(o => o.OrderDate >= dto.DateFrom.Value);
				}

				if (dto.DateTo.HasValue)
				{
					query = query.Where(o => o.OrderDate <= dto.DateTo.Value);
				}

				if (!string.IsNullOrWhiteSpace(dto.Status) && Enum.TryParse<OrderStatus>(dto.Status, true, out var statusEnum))
				{
					query = query.Where(o => o.Status == statusEnum);
				}

				filteredOrders = await query
					.Select(o => new OrderListItemViewModel
					{
						Id = o.Id,
						OrderNumber = o.OrderNumber,
						CustomerName = o.User != null ? o.User.UserName : o.GuestName,
						CustomerEmail = o.User != null ? o.User.Email : o.GuestEmail,
						Date = o.OrderDate,
						Status = o.Status.ToString(),
						Total = o.TotalAmount
					})
					.ToListAsync();
			}

			return filteredOrders;
		}

		public async Task<AdminSalesOverviewViewModel?> GetSaleOverviewAsync(DateTime? startDate, DateTime? endDate)
		{
			AdminSalesOverviewViewModel? model = null;
			
			if (startDate.HasValue && endDate.HasValue)
			{
				var orders = await this._orderRepository
								.GetAllAttached()
								.Where(o => !o.IsCancelled && o.OrderDate >= startDate && o.OrderDate <= endDate)
								.ToListAsync();

				decimal totalRevenue = orders.Sum(o => o.TotalAmount);
				int totalOrdersCount = orders.Count();
				decimal avgTotalRevenue = orders.Count == 0 ? 0 : (totalRevenue / totalOrdersCount);

				var revenueTrends = new RevenueTrendData()
				{
					Labels = orders
									.GroupBy(o => o.OrderDate)
									.OrderBy(g => g.Key)
									.Select(g => g.Key.ToString("MMM dd"))
									.ToList(),
					Values = orders
									.GroupBy(o => o.OrderDate)
									.OrderBy(g => g.Key)
									.Select(g => g.Sum(o => o.TotalAmount))
									.ToList()

				};

				var paymentMethods = new PaymentMethodData()
				{
					Labels = orders
									.Where(o => o.PaymentMethod != null)
									.GroupBy(o => o.PaymentMethod.Name)
									.Select(g => g.Key)
									.ToList(),
					Values = orders
									.Where(o => o.PaymentMethod != null)
									.GroupBy(o => o.PaymentMethod.Name)
									.Select(g => g.Sum(o => o.TotalAmount))
									.ToList()
				};

				model = new AdminSalesOverviewViewModel()
				{
					TotalRevenue = totalRevenue,
					TotalOrders = totalOrdersCount,
					AverageOrderValue = avgTotalRevenue,
					RevenueTrends = revenueTrends,
					PaymentMethods = paymentMethods
				};
			}

			return model;
		}

		public async Task<OrderDetailsViewModel?> GetOrderDetailsAsync(int? orderId)
		{
			OrderDetailsViewModel? details = null;

			if (orderId.HasValue)
			{
				Order? order = await this._orderRepository
								.SingleOrDefaultAsync(o => o.Id == orderId);

				if (order != null)
				{
					details = new OrderDetailsViewModel()
					{
						Id = order.Id,
						OrderNumber = order.OrderNumber,
						OrderDate = order.OrderDate,
						CustomerName = order.User != null ? order.User.UserName : order.GuestName,
						CustomerEmail = order.User != null ? order.User.Email : order.GuestEmail,
						Status = order.Status.ToString(),
						AvailableStatuses = GetOrderStatusses(),
						PaymentMethod = order.PaymentMethod.Name,
						BillingAddress = FormatAddress(order.BillingAddress),
						ShippingAddress = FormatAddress(order.ShippingAddress),
						ShippingOption = order.ShippingOption,
						EstimateDeliveryStart = order.EstimatedDeliveryStart.ToString("MMM dd"),
						EstimateDeliveryEnd = order.EstimatedDeliveryEnd.ToString("MMM dd"),
						Subtotal = order.TotalAmount - order.ShippingPrice,
						ShippingCost = order.ShippingPrice,
						Total = order.TotalAmount,
						CanCancel = !order.IsCancelled,
						CanRefund = order.IsCancelled,
						Items = order.OrderItems.Select(i => new OrderItemViewModel()
						{
							ProductName = i.Product.Name,
							ProductImageUrl = i.Product.ImageUrl,
							ProductSize = i.ProductSize,
							Price = i.UnitPrice,
							Quantity = i.Quantity
						}).ToList()
					};
				}
			}

			return details;
		}

		public async Task<(bool isCancelled, string message)> CancelOrderAsync(int? orderId)
		{
			if (orderId.HasValue)
			{
				Order? order = await this._orderRepository
									.GetByIdAsync(orderId.Value);

				if (order != null)
				{
					if (order.IsCancelled) return (false, "The Order is already cancelled!");

					order.IsCancelled = true;
					order.IsCompleted = false;

					order.Status = OrderStatus.Cancelled;

					bool isUpdated = await this._orderRepository.UpdateAsync(order);

					return (isUpdated, string.Empty);
				}
			}

			return (false, "The OrderId is invalid");
		}

		public async Task<(bool isFinished, string message)> FinishOrderAsync(int? orderId)
		{
			if (orderId.HasValue)
			{
				Order? order = await this._orderRepository
									.GetByIdAsync(orderId.Value);

				if (order != null)
				{
					if (order.IsCompleted) return (false, "The Order is already finished!");

					var today = DateTime.Now;
					if (order.EstimatedDeliveryStart > today || today > order.EstimatedDeliveryEnd)
						return (false, "You cannot finish an Order that is not in the valid delivery Range!");

					order.IsCancelled = false;
					order.IsCompleted = true;

					order.Status = OrderStatus.Delivered;

					bool isUpdated = await this._orderRepository.UpdateAsync(order);

					return (isUpdated, string.Empty);
				}
			}

			return (false, "The OrderId is invalid!");
		}

		public string[] GetOrderStatusses()
		{
			string orderStatusPending = OrderStatus.Pending.ToString();
			string orderStatusDelivered = OrderStatus.Delivered.ToString();
			string orderStatusCancelled = OrderStatus.Cancelled.ToString();


			string[] statusses = { orderStatusPending, orderStatusDelivered, orderStatusCancelled };
			return statusses;
		}

		private static string FormatAddress(Address address)
		{
			StringBuilder str = new StringBuilder();

			string streetAddress = address.Street.ToString();
			string city = address.City.ToString();
			string zipCode = address.ZipCode.ToString();
			string country = address.Country.ToString();
			string phoneNumber = address.PhoneNumber.ToString();

			str.AppendLine(streetAddress);

			str.AppendLine($"{city}, {zipCode}");

			str.AppendLine(country);

			str.Append($"Phone: {phoneNumber}");

			return str.ToString().TrimEnd();
		}

	}
}

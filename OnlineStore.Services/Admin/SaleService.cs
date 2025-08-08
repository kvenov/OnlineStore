using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Services.Core.Admin.Interfaces;
using OnlineStore.Services.Core.DTO.Sales.Overview;

namespace OnlineStore.Services.Core.Admin
{
	public class SaleService : ISaleService
	{
		private readonly IOrderRepository _orderRepository;

		public SaleService(IOrderRepository orderRepository)
		{
			this._orderRepository = orderRepository;
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
	}
}

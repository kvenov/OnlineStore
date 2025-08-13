using MockQueryable;
using Moq;
using OnlineStore.Data.Models;
using OnlineStore.Data.Models.Enums;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Services.Core.Admin;
using OnlineStore.Services.Core.Admin.Interfaces;
using OnlineStore.Services.Core.DTO.Sales.LocationSale;
using OnlineStore.Services.Core.DTO.Sales.OrderManagement;
using OnlineStore.Services.Core.DTO.Sales.ProductAnalytics;

namespace OnlineStore.Services.Tests.Admin
{

	[TestFixture]
	public class AdminSaleServiceTests
	{
		private Mock<IOrderRepository> _mockOrderRepo;
		private Mock<IRepository<OrderItem, int>> _mockOrderItemRepo;
		private IAdminSaleService _service;

		[SetUp]
		public void SetUp()
		{
			_mockOrderRepo = new Mock<IOrderRepository>(MockBehavior.Strict);
			_mockOrderItemRepo = new Mock<IRepository<OrderItem, int>>(MockBehavior.Strict);
			_service = new AdminSaleService(_mockOrderRepo.Object, _mockOrderItemRepo.Object);
		}

		[Test]
		public async Task GetFilteredOrdersAsync_ReturnsNull_WhenDtoIsNull()
		{
			var result = await _service.GetFilteredOrdersAsync(null);
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetFilteredOrdersAsync_ReturnsFilteredOrders_ByOrderNumber()
		{
			var orders = new List<Order>
			{
				new Order { Id = 1, OrderNumber = "A123", User = new ApplicationUser { UserName = "user1", Email = "u1@mail.com" }, OrderDate = DateTime.Today, Status = OrderStatus.Pending, TotalAmount = 100 },
				new Order { Id = 2, OrderNumber = "B456", GuestName = "guest", GuestEmail = "g@mail.com", OrderDate = DateTime.Today, Status = OrderStatus.Delivered, TotalAmount = 200 }
			}.BuildMock();
			_mockOrderRepo.Setup(r => r.GetAllAttached()).Returns(orders);

			var dto = new OrderFilterDto { OrderNumber = "A123" };
			var result = await _service.GetFilteredOrdersAsync(dto);

			Assert.That(result.Count(), Is.EqualTo(1));
			Assert.That(result.First().OrderNumber, Is.EqualTo("A123"));
			Assert.That(result.First().CustomerName, Is.EqualTo("user1"));
			Assert.That(result.First().CustomerEmail, Is.EqualTo("u1@mail.com"));
		}

		[Test]
		public async Task GetFilteredOrdersAsync_FiltersByCustomerAndStatusAndDate()
		{
			var orders = new List<Order>
			{
				new Order { Id = 1, OrderNumber = "A123", User = new ApplicationUser { UserName = "user1", Email = "u1@mail.com" }, OrderDate = DateTime.Today.AddDays(-1), Status = OrderStatus.Pending, TotalAmount = 100 },
				new Order { Id = 2, OrderNumber = "B456", GuestName = "guest", GuestEmail = "g@mail.com", OrderDate = DateTime.Today, Status = OrderStatus.Delivered, TotalAmount = 200 }
			}.BuildMock();
			_mockOrderRepo.Setup(r => r.GetAllAttached()).Returns(orders);

			var dto = new OrderFilterDto
			{
				Customer = "guest",
				Status = "Delivered",
				DateFrom = DateTime.Today.AddDays(-2),
				DateTo = DateTime.Today.AddDays(1)
			};
			var result = await _service.GetFilteredOrdersAsync(dto);

			Assert.That(result.Count(), Is.EqualTo(1));
			Assert.That(result.First().OrderNumber, Is.EqualTo("B456"));
			Assert.That(result.First().Status, Is.EqualTo("Delivered"));
		}

		[Test]
		public async Task GetSaleOverviewAsync_ReturnsNull_WhenDatesAreNull()
		{
			var result = await _service.GetSaleOverviewAsync(null, null);
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetSaleOverviewAsync_ReturnsOverview_WhenOrdersExist()
		{
			var orders = new List<Order>
			{
				new Order { Id = 1, OrderDate = DateTime.Today, IsCancelled = false, TotalAmount = 100, PaymentMethod = new PaymentMethod { Name = "Credit Card" } },
				new Order { Id = 2, OrderDate = DateTime.Today, IsCancelled = false, TotalAmount = 200, PaymentMethod = new PaymentMethod { Name = "PayPal" } }
			};
			var ordersMock = orders.BuildMock();
			_mockOrderRepo.Setup(r => r.GetAllAttached()).Returns(ordersMock);

			var result = await _service.GetSaleOverviewAsync(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1));

			Assert.That(result, Is.Not.Null);
			Assert.That(result.TotalRevenue, Is.EqualTo(300));
			Assert.That(result.TotalOrders, Is.EqualTo(2));
			Assert.That(result.AverageOrderValue, Is.EqualTo(150));
			Assert.That(result.RevenueTrends.Labels.Count, Is.GreaterThan(0));
			Assert.That(result.PaymentMethods.Labels, Contains.Item("Credit Card"));
			Assert.That(result.PaymentMethods.Labels, Contains.Item("PayPal"));
		}

		[Test]
		public async Task GetSaleOverviewAsync_ReturnsZeroes_WhenNoOrders()
		{
			var ordersMock = new List<Order>().BuildMock();
			_mockOrderRepo.Setup(r => r.GetAllAttached()).Returns(ordersMock);

			var result = await _service.GetSaleOverviewAsync(DateTime.Today, DateTime.Today);
			Assert.That(result.TotalRevenue, Is.EqualTo(0));
			Assert.That(result.TotalOrders, Is.EqualTo(0));
			Assert.That(result.AverageOrderValue, Is.EqualTo(0));
		}

		[Test]
		public async Task GetOrderDetailsAsync_ReturnsNull_WhenOrderIdIsNull()
		{
			var result = await _service.GetOrderDetailsAsync(null);
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetOrderDetailsAsync_ReturnsNull_WhenOrderNotFound()
		{
			_mockOrderRepo.Setup(r => r.SingleOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>())).ReturnsAsync((Order)null);

			var result = await _service.GetOrderDetailsAsync(1);
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetOrderDetailsAsync_ReturnsDetails_WhenOrderFound()
		{
			var order = new Order
			{
				Id = 1,
				OrderNumber = "A123",
				OrderDate = DateTime.Today,
				User = new ApplicationUser { UserName = "user1", Email = "u1@mail.com" },
				Status = OrderStatus.Pending,
				PaymentMethod = new PaymentMethod { Name = "Credit Card" },
				BillingAddress = new Address { Street = "Main", City = "City", ZipCode = "12345", Country = "Country", PhoneNumber = "123456789" },
				ShippingAddress = new Address { Street = "Main", City = "City", ZipCode = "12345", Country = "Country", PhoneNumber = "123456789" },
				ShippingOption = "Express",
				EstimatedDeliveryStart = DateTime.Today,
				EstimatedDeliveryEnd = DateTime.Today.AddDays(2),
				TotalAmount = 100,
				ShippingPrice = 10,
				IsCancelled = false,
				OrderItems = new List<OrderItem>
				{
					new OrderItem { Product = new Product { Name = "P1", ImageUrl = "img1", Price = 50 }, ProductSize = "M", UnitPrice = 50, Quantity = 2 }
				}
			};
			_mockOrderRepo.Setup(r => r.SingleOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>())).ReturnsAsync(order);

			var result = await _service.GetOrderDetailsAsync(1);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.OrderNumber, Is.EqualTo("A123"));
			Assert.That(result.CustomerName, Is.EqualTo("user1"));
			Assert.That(result.PaymentMethod, Is.EqualTo("Credit Card"));
			Assert.That(result.BillingAddress, Does.Contain("Main"));
			Assert.That(result.Items.Count, Is.EqualTo(1));
			Assert.That(result.Items.First().ProductName, Is.EqualTo("P1"));
		}

		[Test]
		public async Task CancelOrderAsync_ReturnsFalse_WhenOrderIdIsNull()
		{
			var result = await _service.CancelOrderAsync(null);
			Assert.That(result.isCancelled, Is.False);
			Assert.That(result.message, Does.Contain("invalid"));
		}

		[Test]
		public async Task CancelOrderAsync_ReturnsFalse_WhenOrderNotFound()
		{
			_mockOrderRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Order)null);

			var result = await _service.CancelOrderAsync(1);
			Assert.That(result.isCancelled, Is.False);
			Assert.That(result.message, Does.Contain("invalid"));
		}

		[Test]
		public async Task CancelOrderAsync_ReturnsFalse_WhenOrderAlreadyCancelled()
		{
			var order = new Order { Id = 1, IsCancelled = true };
			_mockOrderRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);

			var result = await _service.CancelOrderAsync(1);
			Assert.That(result.isCancelled, Is.False);
			Assert.That(result.message, Does.Contain("already cancelled"));
		}

		[Test]
		public async Task CancelOrderAsync_ReturnsTrue_WhenOrderCancelled()
		{
			var order = new Order { Id = 1, IsCancelled = false, IsCompleted = true, Status = OrderStatus.Pending };
			_mockOrderRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);
			_mockOrderRepo.Setup(r => r.UpdateAsync(order)).ReturnsAsync(true);

			var result = await _service.CancelOrderAsync(1);
			Assert.That(result.isCancelled, Is.True);
			Assert.That(result.message, Is.Empty);
			Assert.That(order.IsCancelled, Is.True);
			Assert.That(order.Status, Is.EqualTo(OrderStatus.Cancelled));
		}

		[Test]
		public async Task FinishOrderAsync_ReturnsFalse_WhenOrderIdIsNull()
		{
			var result = await _service.FinishOrderAsync(null);
			Assert.That(result.isFinished, Is.False);
			Assert.That(result.message, Does.Contain("invalid"));
		}

		[Test]
		public async Task FinishOrderAsync_ReturnsFalse_WhenOrderNotFound()
		{
			_mockOrderRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Order)null);

			var result = await _service.FinishOrderAsync(1);
			Assert.That(result.isFinished, Is.False);
			Assert.That(result.message, Does.Contain("invalid"));
		}

		[Test]
		public async Task FinishOrderAsync_ReturnsFalse_WhenOrderAlreadyFinished()
		{
			var order = new Order { Id = 1, IsCompleted = true };
			_mockOrderRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);

			var result = await _service.FinishOrderAsync(1);
			Assert.That(result.isFinished, Is.False);
			Assert.That(result.message, Does.Contain("already finished"));
		}

		[Test]
		public async Task FinishOrderAsync_ReturnsFalse_WhenNotInDeliveryRange()
		{
			var order = new Order
			{
				Id = 1,
				IsCompleted = false,
				EstimatedDeliveryStart = DateTime.Now.AddDays(1),
				EstimatedDeliveryEnd = DateTime.Now.AddDays(2)
			};
			_mockOrderRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);

			var result = await _service.FinishOrderAsync(1);
			Assert.That(result.isFinished, Is.False);
			Assert.That(result.message, Does.Contain("not in the valid delivery Range"));
		}

		[Test]
		public async Task FinishOrderAsync_ReturnsTrue_WhenOrderFinished()
		{
			var order = new Order
			{
				Id = 1,
				IsCompleted = false,
				EstimatedDeliveryStart = DateTime.Now.AddDays(-1),
				EstimatedDeliveryEnd = DateTime.Now.AddDays(1),
				Status = OrderStatus.Pending
			};
			_mockOrderRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);
			_mockOrderRepo.Setup(r => r.UpdateAsync(order)).ReturnsAsync(true);

			var result = await _service.FinishOrderAsync(1);
			Assert.That(result.isFinished, Is.True);
			Assert.That(result.message, Is.Empty);
			Assert.That(order.IsCompleted, Is.True);
			Assert.That(order.Status, Is.EqualTo(OrderStatus.Delivered));
		}

		[Test]
		public async Task GetProductAnalyticsAsync_ReturnsEmptyLists_WhenNoOrderItems()
		{
			var orderItemsMock = new List<OrderItem>().BuildMock();
			_mockOrderItemRepo.Setup(r => r.GetAllAttached()).Returns(orderItemsMock);

			var dto = new ProductAnalyticsFilterDto();
			var result = await _service.GetProductAnalyticsAsync(dto);

			Assert.That(result.TopSellingProducts, Is.Empty);
			Assert.That(result.LeastSellingProducts, Is.Empty);
			Assert.That(result.SalesBySize, Is.Empty);
			Assert.That(result.SalesByPriceRange, Is.Empty);
		}

		[Test]
		public async Task GetProductAnalyticsAsync_ReturnsCorrectData_WithFilters()
		{
			var order = new Order { Id = 1, IsCompleted = true, Status = OrderStatus.Delivered, OrderDate = DateTime.Today };
			var orderItems = new List<OrderItem>
			{
				new OrderItem { Order = order, Product = new Product { Name = "P1", Price = 49 }, ProductSize = "M", Quantity = 2 },
				new OrderItem { Order = order, Product = new Product { Name = "P2", Price = 150 }, ProductSize = "L", Quantity = 1 }
			}.BuildMock();
			_mockOrderItemRepo.Setup(r => r.GetAllAttached()).Returns(orderItems);

			var dto = new ProductAnalyticsFilterDto { FromDate = DateTime.Today.AddDays(-1), ToDate = DateTime.Today.AddDays(1), PriceRange = "All" };
			var result = await _service.GetProductAnalyticsAsync(dto);

			Assert.That(result.TopSellingProducts.Count, Is.EqualTo(2));
			Assert.That(result.LeastSellingProducts.Count, Is.EqualTo(2));
			Assert.That(result.SalesBySize.Count, Is.EqualTo(2));
			Assert.That(result.SalesByPriceRange.Count, Is.GreaterThanOrEqualTo(1));
		}

		[Test]
		public async Task GetSalesByLocationAsync_ReturnsEmptyLists_WhenNoOrders()
		{
			var ordersMock = new List<Order>().BuildMock();
			_mockOrderRepo.Setup(r => r.GetAllAttached()).Returns(ordersMock);

			var dto = new SalesByLocationDto();
			var result = await _service.GetSalesByLocationAsync(dto);

			Assert.That(result.SalesByCountry, Is.Empty);
			Assert.That(result.SalesByCity, Is.Empty);
		}

		[Test]
		public async Task GetSalesByLocationAsync_ReturnsCorrectData_WithFilters()
		{
			var address1 = new Address { Country = "BG", City = "Sofia", ZipCode = "1000" };
			var address2 = new Address { Country = "BG", City = "Plovdiv", ZipCode = "4000" };
			var orders = new List<Order>
			{
				new Order { Id = 1, IsCompleted = true, Status = OrderStatus.Delivered, TotalAmount = 100, ShippingAddress = address1, OrderDate = DateTime.Today },
				new Order { Id = 2, IsCompleted = true, Status = OrderStatus.Delivered, TotalAmount = 200, ShippingAddress = address2, OrderDate = DateTime.Today }
			}.BuildMock();
			_mockOrderRepo.Setup(r => r.GetAllAttached()).Returns(orders);

			var dto = new SalesByLocationDto { Country = "BG", City = "Sofia", ZipCode = "1000", FromDate = DateTime.Today.AddDays(-1), ToDate = DateTime.Today.AddDays(1) };
			var result = await _service.GetSalesByLocationAsync(dto);

			Assert.That(result.SalesByCountry.Count, Is.GreaterThanOrEqualTo(1));
			Assert.That(result.SalesByCity.Count, Is.GreaterThanOrEqualTo(1));
			Assert.That(result.SalesByCountry.First().LocationName, Is.EqualTo("BG"));
			Assert.That(result.SalesByCity.First().LocationName, Is.EqualTo("Sofia"));
		}

		[Test]
		public async Task GetCustomerOrderHistoryAsync_ReturnsNull_WhenCustomerIdIsNullOrWhitespace()
		{
			var result1 = await _service.GetCustomerOrderHistoryAsync(null);
			var result2 = await _service.GetCustomerOrderHistoryAsync("");
			var result3 = await _service.GetCustomerOrderHistoryAsync("   ");
			Assert.That(result1, Is.Null);
			Assert.That(result2, Is.Null);
			Assert.That(result3, Is.Null);
		}

		[Test]
		public async Task GetCustomerOrderHistoryAsync_ReturnsOrders_WhenCustomerIdIsValid()
		{
			var orders = new List<Order>
			{
				new Order { OrderNumber = "A123", UserId = "user1", OrderDate = DateTime.Today, Status = OrderStatus.Pending, TotalAmount = 100 },
				new Order { OrderNumber = "B456", GuestId = "guest1", OrderDate = DateTime.Today, Status = OrderStatus.Delivered, TotalAmount = 200 }
			}.BuildMock();

			_mockOrderRepo.Setup(r => r.GetAllAttached()).Returns(orders);

			object? result = await _service.GetCustomerOrderHistoryAsync("user1");
			Assert.That(result, Is.Not.Null);

			var resultList = result as IEnumerable<object>;
			Assert.That(resultList, Is.Not.Null);
			Assert.That(resultList.Count(), Is.EqualTo(1));

			var first = resultList.First();
			var orderNumberProp = first.GetType().GetProperty("OrderNumber");
			var totalAmountProp = first.GetType().GetProperty("Total");

			Assert.That(orderNumberProp, Is.Not.Null);
			Assert.That(totalAmountProp, Is.Not.Null);

			var orderNumberValue = orderNumberProp.GetValue(first) as string;
			var totalAmountValue = (decimal)totalAmountProp.GetValue(first);

			Assert.That(orderNumberValue, Is.EqualTo("A123"));
			Assert.That(totalAmountValue, Is.EqualTo(100));
		}

		[Test]
		public async Task GetCustomersInsights_ReturnsZeroes_WhenNoOrders()
		{
			var ordersMock = new List<Order>().BuildMock();
			_mockOrderRepo.Setup(r => r.GetAllAttached()).Returns(ordersMock);

			var result = await _service.GetCustomersInsights(null, null);
			Assert.That(result.TotalCustomers, Is.EqualTo(0));
			Assert.That(result.RepeatBuyerRate, Is.EqualTo(0));
			Assert.That(result.AverageLtv, Is.EqualTo(0));
			Assert.That(result.TopCustomers, Is.Empty);
		}

		[Test]
		public async Task GetCustomersInsights_ReturnsCorrectData_WithOrders()
		{
			var orders = new List<Order>
			{
				new Order { UserId = "user1", User = new ApplicationUser { UserName = "User1" }, TotalAmount = 100, IsCompleted = true, Status = OrderStatus.Delivered, OrderDate = DateTime.Today },
				new Order { UserId = "user1", User = new ApplicationUser { UserName = "User1" }, TotalAmount = 200, IsCompleted = true, Status = OrderStatus.Delivered, OrderDate = DateTime.Today },
				new Order { GuestId = "guest1", GuestName = "Guest1", TotalAmount = 50, IsCompleted = true, Status = OrderStatus.Delivered, OrderDate = DateTime.Today }
			}.BuildMock();
			_mockOrderRepo.Setup(r => r.GetAllAttached()).Returns(orders);

			var result = await _service.GetCustomersInsights(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1));
			Assert.That(result.TotalCustomers, Is.EqualTo(2));
			Assert.That(result.RepeatBuyerRate, Is.EqualTo(50)); // 1 repeat buyer out of 2
			Assert.That(result.AverageLtv, Is.EqualTo(175)); // (100+200+50)/2
			Assert.That(result.TopCustomers.Count, Is.EqualTo(2));
			Assert.That(result.TopCustomers.Any(c => c.Name == "User1"), Is.True);
			Assert.That(result.TopCustomers.Any(c => c.Name == "Guest1"), Is.True);
		}
	}
}

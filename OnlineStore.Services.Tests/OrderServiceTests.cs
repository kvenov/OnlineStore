using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using OnlineStore.Data.Models;
using OnlineStore.Data.Models.Enums;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Services.Core;
using OnlineStore.Services.Core.Email.Interfaces;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.Order;
using System.Linq.Expressions;

namespace OnlineStore.Services.Tests
{
	[TestFixture]
	public class OrderServiceTests
	{
		private Mock<IOrderRepository> _orderRepoMock;
		private Mock<IShoppingCartRepository> _shoppingCartRepoMock;
		private Mock<UserManager<ApplicationUser>> _userManagerMock;
		private Mock<IRepository<OrderItem, int>> _syncOrderItemRepoMock;
		private Mock<IAsyncRepository<OrderItem, int>> _asyncOrderItemRepoMock;
		private Mock<ICheckoutRepository> _checkoutRepoMock;
		private Mock<IProductRepository> _productRepoMock;
		private Mock<IOrderEmailService> _orderEmailService;
		private Mock<ILogger<OrderService>> _logger;

		private IOrderService _orderService;

		[SetUp]
		public void SetUp()
		{
			_orderRepoMock = new Mock<IOrderRepository>();
			_shoppingCartRepoMock = new Mock<IShoppingCartRepository>();
			_userManagerMock = MockUserManager();
			_syncOrderItemRepoMock = new Mock<IRepository<OrderItem, int>>();
			_asyncOrderItemRepoMock = new Mock<IAsyncRepository<OrderItem, int>>();
			_checkoutRepoMock = new Mock<ICheckoutRepository>();
			_productRepoMock = new Mock<IProductRepository>();
			_orderEmailService = new Mock<IOrderEmailService>();
			_logger = new Mock<ILogger<OrderService>>();

			_orderService = new OrderService(
				_orderRepoMock.Object,
				_shoppingCartRepoMock.Object,
				_userManagerMock.Object,
				_syncOrderItemRepoMock.Object,
				_asyncOrderItemRepoMock.Object,
				_checkoutRepoMock.Object,
				_productRepoMock.Object,
				_orderEmailService.Object,
				_logger.Object
			);
		}

		private static Mock<UserManager<ApplicationUser>> MockUserManager()
		{
			var store = new Mock<IUserStore<ApplicationUser>>();
			return new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
		}

		private Checkout CreateCheckout(bool isRegisteredUser)
		{
			return new Checkout
			{
				Id = 10,
				UserId = isRegisteredUser ? "user-123" : null,
				GuestId = isRegisteredUser ? null : "guest-999",
				GuestEmail = isRegisteredUser ? null : "guest@example.com",
				GuestName = isRegisteredUser ? null : "Guest Name",
				TotalPrice = 100,
				ShippingAddressId = 1,
				BillingAddressId = 2,
				PaymentMethodId = 5,
				PaymentDetails = new PaymentDetails { NameOnCard = "John Doe", CardNumber = "1234567890123456" },
				ShippingOption = "Express",
				EstimatedDeliveryStart = DateTime.UtcNow,
				EstimatedDeliveryEnd = DateTime.UtcNow.AddDays(3),
				ShippingPrice = 10,
				ShoppingCart = new ShoppingCart
				{
					Id = 50,
					ShoppingCartItems = new List<ShoppingCartItem>
					{
						new ShoppingCartItem
						{
							ProductId = 1,
							Quantity = 2,
							Price = 25,
							TotalPrice = 50,
							ProductSize = "M"
						},
						new ShoppingCartItem
						{
							ProductId = 2,
							Quantity = 1,
							Price = 50,
							TotalPrice = 50,
							ProductSize = "L"
						}
					}
				}
			};
		}

		private Order CreateOrder(
			string userId = "u1",
			string guestId = null,
			string guestEmail = null)
		{
			var user = userId != null
				? new ApplicationUser { Id = userId, UserName = "testuser" }
				: null;

			var order = new Order
			{
				Id = 1,
				UserId = userId,
				User = user,
				GuestId = guestId,
				GuestEmail = guestEmail,
				OrderNumber = "ORD123",
				OrderDate = new DateTime(2025, 1, 1),
				EstimatedDeliveryStart = new DateTime(2025, 1, 5),
				EstimatedDeliveryEnd = new DateTime(2025, 1, 7),
				TotalAmount = 99.99m,
				ShippingOption = "Standard",
				ShippingPrice = 4.99m,
				Status = OrderStatus.Processing,
				IsCompleted = false,
				IsCancelled = false,
				ShippingAddress = new Address
				{
					Street = "123 Main St",
					City = "City",
					Country = "Country",
					ZipCode = "12345",
					PhoneNumber = "555-1234"
				},
				BillingAddress = new Address
				{
					Street = "123 Main St",
					City = "City",
					Country = "Country",
					ZipCode = "12345",
					PhoneNumber = "555-1234"
				},
				PaymentMethod = new PaymentMethod
				{
					Name = "Credit Card",
					Code = PaymentMethodCode.CreditCard
				},
				PaymentDetails = new PaymentDetails
				{
					NameOnCard = "John Doe",
					CardNumber = "4111111111111111",
					ExpMonth = 12,
					ExpYear = 2030,
					Status = PaymentStatus.Paid
				},
				OrderItems = new List<OrderItem>
				{
					new OrderItem
					{
						Id = 1,
						Quantity = 2,
						UnitPrice = 10,
						ProductSize = "M",
						OrderId = 1,
						Product = new Product { Name = "Test Product", ImageUrl = "img.jpg" }
					}
				}
			};

			if (user != null)
			{
				user.Orders.Add(order);
			}

			return order;
		}


		[Test]
		public async Task CreateOrderAsync_WhenCheckoutIsNull_ReturnsNull()
		{
			var result = await _orderService.CreateOrderAsync(null);

			Assert.That(result, Is.Null);
			_orderRepoMock.Verify(x => x.AddAsync(It.IsAny<Order>()), Times.Never);
		}

		[Test]
		public async Task CreateOrderAsync_WithRegisteredUser_AddsOrderAndClearsCart()
		{
			var checkout = CreateCheckout(true);

			_orderRepoMock.Setup(x => x.AddAsync(It.IsAny<Order>()))
						  .Callback<Order>(o => o.Id = 123)
						  .Returns(Task.CompletedTask);

			var result = await _orderService.CreateOrderAsync(checkout);

			Assert.That(result, Is.EqualTo(123));
			Assert.That(checkout.Order, Is.Not.Null);
			Assert.That(checkout.IsCompleted, Is.True);
			Assert.That(checkout.CompletedAt, Is.Not.Null);

			_orderRepoMock.Verify(x => x.AddAsync(It.Is<Order>(o =>
				o.UserId == "user-123" &&
				o.TotalAmount == checkout.TotalPrice &&
				o.OrderItems.Count() == checkout.ShoppingCart.ShoppingCartItems.Count
			)), Times.Once);

			_shoppingCartRepoMock.Verify(x => x.ClearShoppingCartItemsAsync(50), Times.Once);
			_checkoutRepoMock.Verify(x => x.UpdateAsync(It.Is<Checkout>(c => c.IsCompleted)), Times.Once);
		}

		[Test]
		public async Task CreateOrderAsync_WithGuestUser_AddsOrderAndClearsCart()
		{
			var checkout = CreateCheckout(false);

			_orderRepoMock.Setup(x => x.AddAsync(It.IsAny<Order>()))
						  .Callback<Order>(o => o.Id = 321)
						  .Returns(Task.CompletedTask);

			var result = await _orderService.CreateOrderAsync(checkout);

			Assert.That(result, Is.EqualTo(321));
			Assert.That(checkout.Order, Is.Not.Null);
			Assert.That(checkout.IsCompleted, Is.True);

			_orderRepoMock.Verify(x => x.AddAsync(It.Is<Order>(o =>
				o.GuestId == "guest-999" &&
				o.GuestEmail == "guest@example.com" &&
				o.OrderItems.Count() == checkout.ShoppingCart.ShoppingCartItems.Count
			)), Times.Once);

			_shoppingCartRepoMock.Verify(x => x.ClearShoppingCartItemsAsync(50), Times.Once);
			_checkoutRepoMock.Verify(x => x.UpdateAsync(It.IsAny<Checkout>()), Times.Once);
		}

		[Test]
		public async Task GetUserOrdersAsync_WhenUserIdIsNull_ReturnsNull()
		{
			var result = await _orderService.GetUserOrdersAsync(null);

			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetUserOrdersAsync_WhenUserNotFound_ReturnsNull()
		{
			_userManagerMock.Setup(x => x.FindByIdAsync("uid")).ReturnsAsync((ApplicationUser)null);

			var result = await _orderService.GetUserOrdersAsync("uid");

			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetUserOrdersAsync_WhenUserExistsButNoOrders_ReturnsEmpty()
		{
			var user = new ApplicationUser { Id = "u1" };
			_userManagerMock.Setup(x => x.FindByIdAsync("u1")).ReturnsAsync(user);

			var orders = new List<Order>().BuildMock();
			_orderRepoMock.Setup(x => x.GetAllAttached()).Returns(orders);

			var result = await _orderService.GetUserOrdersAsync("u1");

			Assert.That(result, Is.Empty);
		}

		[Test]
		public async Task GetUserOrdersAsync_WhenUserExistsWithOrders_ReturnsMappedModels()
		{
			var user = new ApplicationUser { Id = "u1" };
			_userManagerMock.Setup(x => x.FindByIdAsync("u1")).ReturnsAsync(user);

			var order1 = CreateOrder("u1");
			order1.OrderDate = new DateTime(2025, 1, 1);

			var order2 = CreateOrder("u1");
			order2.OrderDate = new DateTime(2024, 12, 31);

			var orders = new List<Order> { order1, order2 }
				.BuildMock();

			_orderRepoMock.Setup(x => x.GetAllAttached()).Returns(orders);


			var result = await _orderService.GetUserOrdersAsync("u1");

			Assert.That(result.ToList(), Has.Count.EqualTo(2));
			Assert.That(result.First().OrderDate, Is.EqualTo(new DateTime(2025, 1, 1)));
			Assert.That(result.First().Items.First().Name, Is.EqualTo("Test Product"));
		}

		[Test]
		public async Task GetGuestOrderAsync_WhenModelIsNull_ReturnsFalse()
		{
			var result = await _orderService.GetGuestOrderAsync("gid", null);

			Assert.That(result, Is.False);
		}

		[Test]
		public async Task GetGuestOrderAsync_WhenFormInvalid_ReturnsFalseAndSetsHasSearched()
		{
			var model = new TrackGuestOrderViewModel { GuestEmail = "", OrderNumber = "" };

			var result = await _orderService.GetGuestOrderAsync("", model);

			Assert.That(result, Is.False);
			Assert.That(model.HasSearched, Is.True);
		}

		[Test]
		public async Task GetGuestOrderAsync_WhenOrderNotFound_ReturnsFalse()
		{
			var model = new TrackGuestOrderViewModel { GuestEmail = "g@x.com", OrderNumber = "ORD1" };
			var orders = new List<Order>().BuildMock();

			_orderRepoMock.Setup(x => x.GetAllAttached()).Returns(orders);

			var result = await _orderService.GetGuestOrderAsync("gid", model);

			Assert.That(result, Is.False);
			Assert.That(model.HasSearched, Is.True);
		}

		[Test]
		public async Task GetGuestOrderAsync_WhenGuestIdMismatch_ReturnsFalse()
		{
			var model = new TrackGuestOrderViewModel { GuestEmail = "g@x.com", OrderNumber = "ord123" };

			var order = CreateOrder("u1", "wrongGuest", "g@x.com");
			var orders = new List<Order> { order }.BuildMock();

			_orderRepoMock.Setup(x => x.GetAllAttached()).Returns(orders);

			var result = await _orderService.GetGuestOrderAsync("gid", model);

			Assert.That(result, Is.False);
			Assert.That(model.HasSearched, Is.True);
		}

		[Test]
		public async Task GetGuestOrderAsync_WhenOrderFoundAndGuestIdMatches_ReturnsTrueAndPopulatesModel()
		{
			var model = new TrackGuestOrderViewModel { GuestEmail = "g@x.com", OrderNumber = "ord123" };

			var order = CreateOrder(null, "gid", "g@x.com");
			var orders = new List<Order> { order }.BuildMock();

			_orderRepoMock.Setup(x => x.GetAllAttached()).Returns(orders);

			var result = await _orderService.GetGuestOrderAsync("gid", model);

			Assert.That(result, Is.True);
			Assert.That(model.OrderFound, Is.True);
			Assert.That(model.Status, Is.EqualTo(order.Status.ToString()));
			Assert.That(model.Items.First().Name, Is.EqualTo("Test Product"));
			Assert.That(model.EstimatedDeliveryStartFormatted, Is.EqualTo("Jan 05"));
			Assert.That(model.EstimatedDeliveryEndFormatted, Is.EqualTo("Jan 07"));
		}

		[Test]
		public async Task GetOrderDetailsAsync_WhenOrderExists_ReturnsMappedModel()
		{
			// Arrange
			var order = CreateOrder();
			_orderRepoMock
				.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>()))
				.ReturnsAsync(order);

			// Act
			var result = await _orderService.GetOrderDetailsAsync("ORD123");

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.OrderNumber, Is.EqualTo("ORD123"));
			Assert.That(result.Items, Has.Count.EqualTo(1));
			Assert.That(result.PaymentDetails!.NameOnCard, Is.EqualTo("John Doe"));
			Assert.That(result.ShippingAddress.City, Is.EqualTo("City"));
		}

		[Test]
		public async Task GetOrderForConfirmationPageAsync_WhenUserOwnsOrder_ReturnsConfirmationModel()
		{
			var order = CreateOrder();
			order.User!.Orders.Add(order);

			_orderRepoMock
				.Setup(r => r.GetByIdAsync(order.Id))
				.ReturnsAsync(order);

			var orderItems = order.OrderItems.ToArray();
			_syncOrderItemRepoMock
				.Setup(r => r.GetAllAttached())
				.Returns(orderItems.BuildMock());

			_userManagerMock
				.Setup(m => m.FindByIdAsync("u1"))
				.ReturnsAsync(order.User);

			var result = await _orderService.GetOrderForConfirmationPageAsync("u1", 1);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.OrderNumber, Is.EqualTo("ORD123"));
			Assert.That(result.IsGuest, Is.False);
			Assert.That(result.Products, Has.Count.EqualTo(1));
		}

		[Test]
		public async Task GetOrderForConfirmationPageAsync_WhenGuestOwnsOrder_ReturnsGuestConfirmationModel()
		{
			// Arrange
			var order = CreateOrder(userId: null);
			order.GuestId = "guest-1";
			order.GuestName = "Guest User";
			order.GuestEmail = "guest@example.com";

			_orderRepoMock
				.Setup(r => r.GetByIdAsync(order.Id))
				.ReturnsAsync(order);

			var orderItems = order.OrderItems.ToArray();
			_syncOrderItemRepoMock
				.Setup(r => r.GetAllAttached())
				.Returns(orderItems.BuildMock());

			_userManagerMock
				.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
				.ReturnsAsync((ApplicationUser?)null);

			// Act
			var result = await _orderService.GetOrderForConfirmationPageAsync("guest-1", 1);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.IsGuest, Is.True);
			Assert.That(result.RecipientName, Is.EqualTo("Guest User"));
		}

	}
}
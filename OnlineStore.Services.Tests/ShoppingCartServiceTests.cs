using Microsoft.AspNetCore.Identity;
using MockQueryable;
using Moq;
using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Services.Core;
using System.Linq.Expressions;

namespace OnlineStore.Services.Tests
{
	[TestFixture]
	public class ShoppingCartServiceTests
	{
		private Mock<IShoppingCartRepository> _cartRepoMock;
		private Mock<IProductRepository> _productRepoMock;
		private Mock<UserManager<ApplicationUser>> _userManagerMock;

		private ShoppingCartService _service;

		[SetUp]
		public void SetUp()
		{
			_cartRepoMock = new Mock<IShoppingCartRepository>();
			_productRepoMock = new Mock<IProductRepository>();

			var userStore = new Mock<IUserStore<ApplicationUser>>();
			_userManagerMock = new Mock<UserManager<ApplicationUser>>(
				userStore.Object, null, null, null, null, null, null, null, null);

			_service = new ShoppingCartService(
				_userManagerMock.Object,
				_cartRepoMock.Object,
				_productRepoMock.Object
			);
		}

		private ApplicationUser CreateUser(string id = "u1")
			=> new ApplicationUser { Id = id, UserName = "user@test.com", Email = "user@test.com" };

		private Product CreateProduct(int id = 1, string name = "P1", string image = "img1.jpg", decimal price = 20m)
			=> new Product { Id = id, Name = name, ImageUrl = image, Price = price, Description = "d", CreatedAt = DateTime.UtcNow, IsActive = true, StockQuantity = 10, AverageRating = 0, TotalRatings = 0, ProductDetails = new ProductDetails(), Category = new ProductCategory { Id = 1, Name = "C" } };

		private ShoppingCartItem CreateItem(int id, Product product, int qty, decimal unitPrice, string size = "M")
			=> new ShoppingCartItem
			{
				Id = id,
				ProductId = product.Id,
				Product = product,
				Quantity = qty,
				Price = unitPrice,
				TotalPrice = unitPrice * qty,
				ProductSize = size
			};

		private ShoppingCart CreateCartForUser(string userId, IEnumerable<ShoppingCartItem> items)
			=> new ShoppingCart
			{
				Id = 101,
				UserId = userId,
				ShoppingCartItems = items.ToList()
			};

		[Test]
		public async Task GetUserShoppingCartDataAsync_WhenUserIdIsNull_ReturnsEmptyModel()
		{
			// Act
			var result = await _service.GetUserShoppingCartDataAsync(null);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result!.Items, Is.Empty);
			Assert.That(result.Total, Is.EqualTo(0m));
		}

		[Test]
		public async Task GetUserShoppingCartDataAsync_WhenUserNotFound_ReturnsNull()
		{
			// Arrange
			_userManagerMock
				.Setup(m => m.FindByIdAsync("u1"))
				.ReturnsAsync((ApplicationUser)null!);

			// Act
			var result = await _service.GetUserShoppingCartDataAsync("u1");

			// Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetUserShoppingCartDataAsync_WhenUserExistsButNoCart_ReturnsNull()
		{
			// Arrange
			var user = CreateUser("u1");
			_userManagerMock
				.Setup(m => m.FindByIdAsync("u1"))
				.ReturnsAsync(user);

			var empty = new List<ShoppingCart>().BuildMock();
			_cartRepoMock.Setup(r => r.GetAllAttached()).Returns(empty);

			// Act
			var result = await _service.GetUserShoppingCartDataAsync("u1");

			// Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetUserShoppingCartDataAsync_WhenUserAndCartExist_MapsItemsAndTotal()
		{
			// Arrange
			var user = CreateUser("u1");
			_userManagerMock
				.Setup(m => m.FindByIdAsync("u1"))
				.ReturnsAsync(user);

			var p1 = CreateProduct(1, "P1", "img1.jpg", 20m);
			var p2 = CreateProduct(2, "P2", "img2.jpg", 10m);

			var i1 = CreateItem(1, p1, qty: 2, unitPrice: 20m); // 40
			var i2 = CreateItem(2, p2, qty: 1, unitPrice: 10m); // 10

			var cart = CreateCartForUser("u1", new[] { i1, i2 });
			var carts = new List<ShoppingCart> { cart }.BuildMock();
			_cartRepoMock.Setup(r => r.GetAllAttached()).Returns(carts);

			// Act
			var result = await _service.GetUserShoppingCartDataAsync("u1");

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result!.Items, Has.Count.EqualTo(2));
			// Item 1 mapping
			var first = result.Items.First();
			Assert.That(first.Id, Is.EqualTo(1));
			Assert.That(first.ProductId, Is.EqualTo(1));
			Assert.That(first.ProductName, Is.EqualTo("P1"));
			Assert.That(first.ProductImageUrl, Is.EqualTo("img1.jpg"));
			Assert.That(first.ProductSize, Is.EqualTo("M"));
			Assert.That(first.UnitPrice, Is.EqualTo(20m));
			Assert.That(first.Quantity, Is.EqualTo(2));
			Assert.That(first.TotalPrice, Is.EqualTo(40m));
			// Totals
			Assert.That(result.Total, Is.EqualTo(50m));
		}

		[Test]
		public async Task GetShoppingCartForUserAsync_WhenUserIdIsNull_ReturnsEmptyModel()
		{
			// Act
			var result = await _service.GetShoppingCartForUserAsync(null);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result!.Items, Is.Empty);
			Assert.That(result.SubTotal, Is.EqualTo(0m));
			Assert.That(result.Shipping, Is.EqualTo(0m));
			Assert.That(result.Total, Is.EqualTo(0m));
		}

		[Test]
		public async Task GetShoppingCartForUserAsync_WhenUserNotFound_ReturnsNull()
		{
			// Arrange
			_userManagerMock
				.Setup(m => m.FindByIdAsync("u1"))
				.ReturnsAsync((ApplicationUser)null!);

			// Act
			var result = await _service.GetShoppingCartForUserAsync("u1");

			// Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetShoppingCartForUserAsync_WhenUserExistsButNoCart_ReturnsNull()
		{
			// Arrange
			var user = CreateUser("u1");
			_userManagerMock
				.Setup(m => m.FindByIdAsync("u1"))
				.ReturnsAsync(user);

			var empty = new List<ShoppingCart>().BuildMock();
			_cartRepoMock.Setup(r => r.GetAllAttached()).Returns(empty);

			// Act
			var result = await _service.GetShoppingCartForUserAsync("u1");

			// Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetShoppingCartForUserAsync_WhenUserAndCartExist_MapsItemsAndShippingAndTotals()
		{
			// Arrange
			var user = CreateUser("u1");
			_userManagerMock
				.Setup(m => m.FindByIdAsync("u1"))
				.ReturnsAsync(user);

			_cartRepoMock
				.Setup(r => r.GetShoppingCartShippingCostByUserIdAsync("u1"))
				.ReturnsAsync(5.99m);

			var p1 = CreateProduct(1, "P1", "img1.jpg", 20m);
			var p2 = CreateProduct(2, "P2", "img2.jpg", 10m);

			var i1 = CreateItem(1, p1, qty: 2, unitPrice: 20m); // 40
			var i2 = CreateItem(2, p2, qty: 1, unitPrice: 10m); // 10

			var cart = CreateCartForUser("u1", new[] { i1, i2 });
			var carts = new List<ShoppingCart> { cart }.BuildMock();
			_cartRepoMock.Setup(r => r.GetAllAttached()).Returns(carts);

			// Act
			var result = await _service.GetShoppingCartForUserAsync("u1");

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result!.Items, Has.Count.EqualTo(2));

			// Shipping and totals
			Assert.That(result.Shipping, Is.EqualTo(5.99m));
			Assert.That(result.SubTotal, Is.EqualTo(50m));
			Assert.That(result.Total, Is.EqualTo(55.99m));

			// Spot-check item mapping
			var first = result.Items.First();
			Assert.That(first.ProductName, Is.EqualTo("P1"));
			Assert.That(first.ProductImageUrl, Is.EqualTo("img1.jpg"));
			Assert.That(first.UnitPrice, Is.EqualTo(20m));
			Assert.That(first.Quantity, Is.EqualTo(2));
		}

		#region GetUserShoppingCartItemsCountAsync Tests

		[Test]
		public async Task GetUserShoppingCartItemsCountAsync_UserIsNull_ReturnsZero()
		{
			_userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
				.ReturnsAsync((ApplicationUser?)null);

			var result = await _service.GetUserShoppingCartItemsCountAsync("user123");

			Assert.That(result, Is.EqualTo(0));
		}

		[Test]
		public async Task GetUserShoppingCartItemsCountAsync_ShoppingCartIsNull_ReturnsZero()
		{
			var user = new ApplicationUser { Id = "user123" };
			_userManagerMock.Setup(x => x.FindByIdAsync("user123")).ReturnsAsync(user);

			var mockShoppingCarts = new List<ShoppingCart>().BuildMock();
			_cartRepoMock.Setup(x => x.GetAllAttached())
				.Returns(mockShoppingCarts);

			var result = await _service.GetUserShoppingCartItemsCountAsync("user123");

			Assert.That(result, Is.EqualTo(0));
		}

		[Test]
		public async Task GetUserShoppingCartItemsCountAsync_ShoppingCartHasItems_ReturnsCount()
		{
			var user = new ApplicationUser { Id = "user123" };
			var shoppingCart = new ShoppingCart
			{
				Id = 1,
				UserId = "user123",
				ShoppingCartItems = new List<ShoppingCartItem> { new ShoppingCartItem(), new ShoppingCartItem() }
			};

			_userManagerMock.Setup(x => x.FindByIdAsync("user123")).ReturnsAsync(user);

			var mockShoppingCarts = new List<ShoppingCart> { shoppingCart }.BuildMock();
			_cartRepoMock.Setup(x => x.GetAllAttached())
				.Returns(mockShoppingCarts);

			var result = await _service.GetUserShoppingCartItemsCountAsync("user123");

			Assert.That(result, Is.EqualTo(2));
		}

		[Test]
		public async Task GetUserShoppingCartItemsCountAsync_UserIdIsNull_ReturnsZero()
		{
			var result = await _service.GetUserShoppingCartItemsCountAsync(null);
			Assert.That(result, Is.EqualTo(0));
		}

		#endregion

		#region AddToCartForUserAsync Tests

		[Test]
		public async Task AddToCartForUserAsync_NullParameters_ReturnsFalse()
		{
			var result = await _service.AddToCartForUserAsync(null, null, null);
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task AddToCartForUserAsync_ProductOrUserNotFound_ReturnsFalse()
		{
			_productRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product?)null);
			_userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);

			var result = await _service.AddToCartForUserAsync(1, "M", "user123");

			Assert.That(result, Is.False);
		}

		[Test]
		public async Task AddToCartForUserAsync_ShoppingCartIsNull_ReturnsFalse()
		{
			var user = new ApplicationUser { Id = "user123" };
			var product = new Product { Id = 1, Price = 100m };

			_userManagerMock.Setup(x => x.FindByIdAsync("user123")).ReturnsAsync(user);
			_productRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);

			var mockShoppingCarts = new List<ShoppingCart>().BuildMock();
			_cartRepoMock.Setup(x => x.GetAllAttached())
				.Returns(mockShoppingCarts);

			var result = await _service.AddToCartForUserAsync(1, "M", "user123");

			Assert.That(result, Is.False);
		}

		[Test]
		public async Task AddToCartForUserAsync_AddsNewItem_ReturnsTrue()
		{
			var user = new ApplicationUser { Id = "user123" };
			var product = new Product { Id = 1, Price = 100m };
			var shoppingCart = new ShoppingCart { Id = 1, UserId = "user123", ShoppingCartItems = new List<ShoppingCartItem>() };

			_userManagerMock.Setup(x => x.FindByIdAsync("user123")).ReturnsAsync(user);
			_productRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);

			var mockShoppingCarts = new List<ShoppingCart> { shoppingCart }.BuildMock();
			_cartRepoMock.Setup(x => x.GetAllAttached())
				.Returns(mockShoppingCarts);

			_cartRepoMock.Setup(x => x.GetShoppingCartItemAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ShoppingCartItem, bool>>>()))
				.ReturnsAsync((ShoppingCartItem?)null);
			_cartRepoMock.Setup(x => x.AddShoppingCartItemAsync(It.IsAny<ShoppingCartItem>())).Returns(Task.CompletedTask);
			_cartRepoMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

			var result = await _service.AddToCartForUserAsync(1, "M", "user123");

			Assert.That(result, Is.True);
			Assert.That(shoppingCart.ShoppingCartItems.Count, Is.EqualTo(1));
		}

		[Test]
		public async Task AddToCartForUserAsync_AddsNewItem_WhenNoMatchingIdsAndSizes()
		{
			var user = new ApplicationUser { Id = "user123" };
			var product = new Product { Id = 1, Price = 100m };
			var existingItem = new ShoppingCartItem { ProductId = 2, Quantity = 1, ProductSize = "L"};
			var shoppingCart = new ShoppingCart { Id = 1, UserId = "user123", ShoppingCartItems = new List<ShoppingCartItem> { existingItem } };

			_userManagerMock.Setup(x => x.FindByIdAsync("user123")).ReturnsAsync(user);
			_productRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);

			var mockShoppingCarts = new List<ShoppingCart> { shoppingCart }.BuildMock();
			_cartRepoMock.Setup(x => x.GetAllAttached())
				.Returns(mockShoppingCarts);

			_cartRepoMock.Setup(x => x.GetShoppingCartItemAsync(It.IsAny<Expression<System.Func<ShoppingCartItem, bool>>>()))
				.ReturnsAsync((ShoppingCartItem?)null);
			_cartRepoMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

			var result = await _service.AddToCartForUserAsync(1, "M", "user123");

			Assert.That(result, Is.True);
			Assert.That(existingItem.Quantity, Is.EqualTo(1));
			Assert.That(shoppingCart.ShoppingCartItems.Count, Is.EqualTo(2));
		}

		[Test]
		public async Task AddToCartForUserAsync_IncrementsExistingItem_ReturnsTrue()
		{
			var user = new ApplicationUser { Id = "user123" };
			var product = new Product { Id = 1, Price = 100m };
			var existingItem = new ShoppingCartItem { ProductId = 1, Quantity = 1, ProductSize = "M"};
			var shoppingCart = new ShoppingCart { Id = 1, UserId = "user123", ShoppingCartItems = new List<ShoppingCartItem> { existingItem } };

			_userManagerMock.Setup(x => x.FindByIdAsync("user123")).ReturnsAsync(user);
			_productRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);

			var mockShoppingCarts = new List<ShoppingCart> { shoppingCart }.BuildMock();
			_cartRepoMock.Setup(x => x.GetAllAttached())
				.Returns(mockShoppingCarts);

			_cartRepoMock.Setup(x => x.GetShoppingCartItemAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ShoppingCartItem, bool>>>()))
				.ReturnsAsync(existingItem);
			_cartRepoMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

			var result = await _service.AddToCartForUserAsync(1, "M", "user123");

			Assert.That(result, Is.True);
			Assert.That(existingItem.Quantity, Is.EqualTo(2));
			Assert.That(shoppingCart.ShoppingCartItems.Count, Is.EqualTo(1));
		}

		#endregion

		#region AddToCartForGuestAsync Tests

		[Test]
		public async Task AddToCartForGuestAsync_NullParameters_ReturnsFalse()
		{
			var result = await _service.AddToCartForGuestAsync(null, null, null);
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task AddToCartForGuestAsync_ProductNotFound_ReturnsFalse()
		{
			_productRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product?)null);

			var result = await _service.AddToCartForGuestAsync(1, "M", "guest123");

			Assert.That(result, Is.False);
		}

		[Test]
		public async Task AddToCartForGuestAsync_AddsNewCartAndItem_ReturnsTrue()
		{
			var product = new Product { Id = 1, Price = 50m };
			var shoppingCartList = new List<ShoppingCart>();

			_productRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);

			var mockShoppingCarts = new List<ShoppingCart>().BuildMock();
			_cartRepoMock.Setup(x => x.GetAllAttached())
				.Returns(mockShoppingCarts);

			_cartRepoMock.Setup(x => x.AddAsync(It.IsAny<ShoppingCart>()))
				.Callback<ShoppingCart>(sc => shoppingCartList.Add(sc))
				.Returns(Task.CompletedTask);
			_cartRepoMock.Setup(x => x.AddShoppingCartItemAsync(It.IsAny<ShoppingCartItem>())).Returns(Task.CompletedTask);
			_cartRepoMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

			var result = await _service.AddToCartForGuestAsync(1, "M", "guest123");

			Assert.That(result, Is.True);
			Assert.That(shoppingCartList.Count, Is.EqualTo(1));
			Assert.That(shoppingCartList[0].ShoppingCartItems.Count, Is.EqualTo(1));
		}

		[Test]
		public async Task AddToCartForGuestAsync_AddsNewItem_WhenNoMatchingIdsAndSizes()
		{
			var product = new Product { Id = 1, Price = 100m };
			var existingItem = new ShoppingCartItem { ProductId = 2, Quantity = 1, ProductSize = "L" };
			var shoppingCart = new ShoppingCart { Id = 1, GuestId = "guest123", ShoppingCartItems = new List<ShoppingCartItem> { existingItem } };

			_productRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);

			var mockShoppingCarts = new List<ShoppingCart> { shoppingCart }.BuildMock();
			_cartRepoMock.Setup(x => x.GetAllAttached())
				.Returns(mockShoppingCarts);

			_cartRepoMock.Setup(x => x.GetShoppingCartItemAsync(It.IsAny<Expression<System.Func<ShoppingCartItem, bool>>>()))
				.ReturnsAsync((ShoppingCartItem?)null);
			_cartRepoMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

			var result = await _service.AddToCartForGuestAsync(1, "M", "guest123");

			Assert.That(result, Is.True);
			Assert.That(existingItem.Quantity, Is.EqualTo(1));
			Assert.That(shoppingCart.ShoppingCartItems.Count, Is.EqualTo(2));
		}

		[Test]
		public async Task AddToCartForGuestAsync_IncrementsExistingItem_ReturnsTrue()
		{
			var product = new Product { Id = 1, Price = 50m };
			var existingItem = new ShoppingCartItem { ProductId = 1, Quantity = 1, ProductSize = "M"};
			var shoppingCart = new ShoppingCart { GuestId = "guest123", ShoppingCartItems = new List<ShoppingCartItem> { existingItem } };

			_productRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);

			var mockShoppingCarts = new List<ShoppingCart> { shoppingCart }.BuildMock();
			_cartRepoMock.Setup(x => x.GetAllAttached())
				.Returns(mockShoppingCarts);

			_cartRepoMock.Setup(x => x.GetShoppingCartItemAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ShoppingCartItem, bool>>>()))
				.ReturnsAsync(existingItem);
			_cartRepoMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

			var result = await _service.AddToCartForGuestAsync(1, "M", "guest123");

			Assert.That(result, Is.True);
			Assert.That(existingItem.Quantity, Is.EqualTo(2));
			Assert.That(shoppingCart.ShoppingCartItems.Count, Is.EqualTo(1));
		}

		#endregion

		#region UpdateUserCartItemAsync Tests

		[Test]
		public async Task UpdateUserCartItemAsync_NullParameters_ReturnsNull()
		{
			var result = await _service.UpdateUserCartItemAsync(null, null, null);
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task UpdateUserCartItemAsync_UserNotFound_ReturnsNull()
		{
			_userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);

			var result = await _service.UpdateUserCartItemAsync("user123", 1, 1);

			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task UpdateUserCartItemAsync_ShoppingCartNotFound_ReturnsNull()
		{
			var user = new ApplicationUser { Id = "user123" };
			_userManagerMock.Setup(x => x.FindByIdAsync("user123")).ReturnsAsync(user);

			_cartRepoMock.Setup(x => x.GetAllAttached())
				.Returns(new List<ShoppingCart>().BuildMock());

			var result = await _service.UpdateUserCartItemAsync("user123", 2, 1);

			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task UpdateUserCartItemAsync_ItemNotFound_ReturnsNull()
		{
			var user = new ApplicationUser { Id = "user123" };
			var cart = new ShoppingCart { Id = 1, UserId = "user123", ShoppingCartItems = new List<ShoppingCartItem>() };

			_userManagerMock.Setup(x => x.FindByIdAsync("user123")).ReturnsAsync(user);
			_cartRepoMock.Setup(x => x.GetAllAttached())
				.Returns(new List<ShoppingCart> { cart }.BuildMock());

			_cartRepoMock.Setup(x => x.GetAllShoppingCartItemsAttached())
				.Returns(new List<ShoppingCartItem>().BuildMock());

			var result = await _service.UpdateUserCartItemAsync("user123", 2, 1);

			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task UpdateUserCartItemAsync_Success_ReturnsSummary()
		{
			var user = new ApplicationUser { Id = "user123" };
			var item = new ShoppingCartItem { Id = 1, Price = 50, Quantity = 1, TotalPrice = 50 };
			var cart = new ShoppingCart { Id = 1, UserId = "user123", ShoppingCartItems = new List<ShoppingCartItem> { item } };

			_userManagerMock.Setup(x => x.FindByIdAsync("user123")).ReturnsAsync(user);
			_cartRepoMock.Setup(x => x.GetAllAttached())
				.Returns(new List<ShoppingCart> { cart }.BuildMock());

			_cartRepoMock.Setup(x => x.GetAllShoppingCartItemsAttached())
				.Returns(new List<ShoppingCartItem> { item }.BuildMock());

			_cartRepoMock.Setup(x => x.GetShoppingCartShippingCostByUserIdAsync("user123")).ReturnsAsync(10);
			_cartRepoMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

			var result = await _service.UpdateUserCartItemAsync("user123", 3, 1);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.ItemTotalPrice, Is.EqualTo(150));
			Assert.That(result.SubTotal, Is.EqualTo(150));
			Assert.That(result.Shipping, Is.EqualTo(10));
			Assert.That(result.Total, Is.EqualTo(160));
		}

		#endregion

		#region RemoveUserCartItemAsync Tests

		[Test]
		public async Task RemoveUserCartItemAsync_NullParameters_ReturnsNull()
		{
			var result = await _service.RemoveUserCartItemAsync(null, null);
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task RemoveUserCartItemAsync_UserNotFound_ReturnsNull()
		{
			_userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);

			var result = await _service.RemoveUserCartItemAsync("user123", 1);

			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task RemoveUserCartItemAsync_ShoppingCartNotFound_ReturnsNull()
		{
			var user = new ApplicationUser { Id = "user123" };
			_userManagerMock.Setup(x => x.FindByIdAsync("user123")).ReturnsAsync(user);

			_cartRepoMock.Setup(x => x.GetAllAttached())
				.Returns(new List<ShoppingCart>().BuildMock());

			var result = await _service.RemoveUserCartItemAsync("user123", 1);

			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task RemoveUserCartItemAsync_ItemExists_RemovesItemAndReturnsSummary()
		{
			var user = new ApplicationUser { Id = "user123" };
			var item = new ShoppingCartItem { Id = 1, Price = 50, Quantity = 1, TotalPrice = 50 };
			var cart = new ShoppingCart { Id = 1, UserId = "user123", ShoppingCartItems = new List<ShoppingCartItem> { item } };

			_userManagerMock.Setup(x => x.FindByIdAsync("user123")).ReturnsAsync(user);
			_cartRepoMock.Setup(x => x.GetAllAttached())
				.Returns(new List<ShoppingCart> { cart }.BuildMock());

			_cartRepoMock.Setup(x => x.GetAllShoppingCartItemsAttached())
				.Returns(new List<ShoppingCartItem> { item }.BuildMock());

			_cartRepoMock.Setup(x => x.GetShoppingCartShippingCostByUserIdAsync("user123")).ReturnsAsync(5);
			_cartRepoMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

			var result = await _service.RemoveUserCartItemAsync("user123", 1);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.SubTotal, Is.EqualTo(0));
			Assert.That(result.Shipping, Is.EqualTo(5));
		}

		#endregion

		#region GetGuestShoppingCartDataAsync Tests

		[Test]
		public async Task GetGuestShoppingCartDataAsync_NullGuestId_ReturnsEmptyCartModel()
		{
			var result = await _service.GetGuestShoppingCartDataAsync(null);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Items, Is.Empty);
			Assert.That(result.Total, Is.EqualTo(0));
		}

		[Test]
		public async Task GetGuestShoppingCartDataAsync_GuestHasCart_ReturnsCartData()
		{
			var product = new Product { Id = 1, Name = "Test Product", ImageUrl = "img.jpg" };
			var item = new ShoppingCartItem
			{
				Id = 1,
				Product = product,
				ProductId = 1,
				Price = 50,
				Quantity = 2,
				TotalPrice = 100,
				ProductSize = "M"
			};
			var cart = new ShoppingCart
			{
				GuestId = "guest123",
				ShoppingCartItems = new List<ShoppingCartItem> { item }
			};

			var cartList = new List<ShoppingCart> { cart };

			// Mock repository to return in-memory IQueryable
			_cartRepoMock.Setup(x => x.GetAllAttached())
				.Returns(cartList.BuildMock());

			var result = await _service.GetGuestShoppingCartDataAsync("guest123");

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Items.Count, Is.EqualTo(1));
			Assert.That(result.Items[0].Id, Is.EqualTo(1));
			Assert.That(result.Items[0].ProductName, Is.EqualTo("Test Product"));
			Assert.That(result.Total, Is.EqualTo(100));
		}

		#endregion

		#region GetGuestShoppingCartItemsCountAsync Tests

		[Test]
		public async Task GetGuestShoppingCartItemsCountAsync_NullGuestId_ReturnsZero()
		{
			var result = await _service.GetGuestShoppingCartItemsCountAsync(null);
			Assert.That(result, Is.EqualTo(0));
		}

		[Test]
		public async Task GetGuestShoppingCartItemsCountAsync_NoCart_ReturnsZero()
		{
			_cartRepoMock.Setup(x => x.GetAllAttached())
				.Returns(new List<ShoppingCart>().BuildMock());

			var result = await _service.GetGuestShoppingCartItemsCountAsync("guest123");

			Assert.That(result, Is.EqualTo(0));
		}

		[Test]
		public async Task GetGuestShoppingCartItemsCountAsync_CartExists_ReturnsItemCount()
		{
			var cart = new ShoppingCart
			{
				GuestId = "guest123",
				ShoppingCartItems = new List<ShoppingCartItem>
			{
				new ShoppingCartItem(),
				new ShoppingCartItem()
			}
			};

			_cartRepoMock.Setup(x => x.GetAllAttached())
				.Returns(new List<ShoppingCart> { cart }.BuildMock());

			var result = await _service.GetGuestShoppingCartItemsCountAsync("guest123");

			Assert.That(result, Is.EqualTo(2));
		}

		#endregion

		#region GetShoppingCartForGuestAsync Tests

		[Test]
		public async Task GetShoppingCartForGuestAsync_NullGuestId_ReturnsEmptyCartModel()
		{
			var result = await _service.GetShoppingCartForGuestAsync(null);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Items, Is.Empty);
			Assert.That(result.Shipping, Is.EqualTo(0));
		}

		[Test]
		public async Task GetShoppingCartForGuestAsync_GuestHasCart_ReturnsCartData()
		{
			var product = new Product { Id = 1, Name = "Test Product", ImageUrl = "img.jpg" };
			var item = new ShoppingCartItem
			{
				Id = 1,
				Product = product,
				ProductId = 1,
				Price = 50,
				Quantity = 2,
				TotalPrice = 100,
				ProductSize = "M"
			};
			var cart = new ShoppingCart
			{
				GuestId = "guest123",
				ShoppingCartItems = new List<ShoppingCartItem> { item }
			};

			_cartRepoMock.Setup(x => x.GetAllAttached())
				.Returns(new List<ShoppingCart> { cart }.BuildMock());
			_cartRepoMock.Setup(x => x.GetShoppingCartShippingCostByUserIdAsync("guest123"))
				.ReturnsAsync(10);

			var result = await _service.GetShoppingCartForGuestAsync("guest123");

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Items.Count, Is.EqualTo(1));
			Assert.That(result.Items[0].Id, Is.EqualTo(1));
			Assert.That(result.Items[0].ProductName, Is.EqualTo("Test Product"));
			Assert.That(result.Shipping, Is.EqualTo(10));
		}

		#endregion

		#region UpdateGuestCartItemAsync Tests

		[Test]
		public async Task UpdateGuestCartItemAsync_NullParameters_ReturnsNull()
		{
			var result = await _service.UpdateGuestCartItemAsync(null, null, null);
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task UpdateGuestCartItemAsync_CartNotFound_ReturnsNull()
		{
			_cartRepoMock.Setup(x => x.GetAllAttached())
				.Returns(new List<ShoppingCart>().BuildMock());

			var result = await _service.UpdateGuestCartItemAsync("guest123", 2, 1);
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task UpdateGuestCartItemAsync_ItemNotFound_ReturnsNull()
		{
			var cart = new ShoppingCart
			{
				GuestId = "guest123",
				ShoppingCartItems = new List<ShoppingCartItem>()
			};

			_cartRepoMock.Setup(x => x.GetAllAttached())
				.Returns(new List<ShoppingCart> { cart }.BuildMock());
			_cartRepoMock.Setup(x => x.GetAllShoppingCartItemsAttached())
				.Returns(new List<ShoppingCartItem>().BuildMock());

			var result = await _service.UpdateGuestCartItemAsync("guest123", 2, 1);
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task UpdateGuestCartItemAsync_Success_ReturnsSummary()
		{
			var item = new ShoppingCartItem { Id = 1, Price = 50, Quantity = 1, TotalPrice = 50 };
			var cart = new ShoppingCart
			{
				GuestId = "guest123",
				ShoppingCartItems = new List<ShoppingCartItem> { item }
			};

			_cartRepoMock.Setup(x => x.GetAllAttached())
				.Returns(new List<ShoppingCart> { cart }.BuildMock());
			_cartRepoMock.Setup(x => x.GetAllShoppingCartItemsAttached())
				.Returns(new List<ShoppingCartItem> { item }.BuildMock());
			_cartRepoMock.Setup(x => x.GetShoppingCartShippingCostByUserIdAsync("guest123"))
				.ReturnsAsync(10);
			_cartRepoMock.Setup(x => x.SaveChangesAsync())
				.Returns(Task.CompletedTask);

			var result = await _service.UpdateGuestCartItemAsync("guest123", 3, 1);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.ItemTotalPrice, Is.EqualTo(150));
			Assert.That(result.SubTotal, Is.EqualTo(150));
			Assert.That(result.Shipping, Is.EqualTo(10));
			Assert.That(result.Total, Is.EqualTo(160));
		}

		#endregion

		#region RemoveGuestCartItemAsync Tests

		[Test]
		public async Task RemoveGuestCartItemAsync_NullGuestIdOrItemId_ReturnsNull()
		{
			var result = await _service.RemoveGuestCartItemAsync(null, null);
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task RemoveGuestCartItemAsync_CartNotFound_ReturnsNull()
		{
			_cartRepoMock.Setup(x => x.GetAllAttached())
				.Returns(new List<ShoppingCart>().BuildMock());

			var result = await _service.RemoveGuestCartItemAsync("guest123", 1);
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task RemoveGuestCartItemAsync_ItemNotFound_ReturnsSummaryWithSubtotal()
		{
			var cart = new ShoppingCart
			{
				GuestId = "guest123",
				ShoppingCartItems = new List<ShoppingCartItem>
			{
				new ShoppingCartItem { Id = 2, Price = 50, Quantity = 1, TotalPrice = 50 }
			}
			};

			_cartRepoMock.Setup(x => x.GetAllAttached())
				.Returns(new List<ShoppingCart> { cart }.BuildMock());
			_cartRepoMock.Setup(x => x.GetAllShoppingCartItemsAttached())
				.Returns(new List<ShoppingCartItem>().BuildMock());
			_cartRepoMock.Setup(x => x.GetShoppingCartShippingCostByUserIdAsync("guest123"))
				.ReturnsAsync(10);
			_cartRepoMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

			var result = await _service.RemoveGuestCartItemAsync("guest123", 1);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.SubTotal, Is.EqualTo(50));
			Assert.That(result.Shipping, Is.EqualTo(10));
		}

		[Test]
		public async Task RemoveGuestCartItemAsync_ItemFound_RemovesItemAndReturnsSummary()
		{
			var item = new ShoppingCartItem { Id = 1, Price = 50, Quantity = 2, TotalPrice = 100 };
			var cart = new ShoppingCart
			{
				GuestId = "guest123",
				ShoppingCartItems = new List<ShoppingCartItem> { item }
			};

			_cartRepoMock.Setup(x => x.GetAllAttached())
				.Returns(new List<ShoppingCart> { cart }.BuildMock());
			_cartRepoMock.Setup(x => x.GetAllShoppingCartItemsAttached())
				.Returns(new List<ShoppingCartItem> { item }.BuildMock());
			_cartRepoMock.Setup(x => x.RemoveShoppingCartItem(item));
			_cartRepoMock.Setup(x => x.GetShoppingCartShippingCostByUserIdAsync("guest123"))
				.ReturnsAsync(10);
			_cartRepoMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

			var result = await _service.RemoveGuestCartItemAsync("guest123", 1);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.SubTotal, Is.EqualTo(0));
			Assert.That(result.Shipping, Is.EqualTo(10));
		}

		#endregion

		#region EnsureGuestCartExistsAsync Tests

		[Test]
		public async Task EnsureGuestCartExistsAsync_CartExists_DoesNotAdd()
		{
			var existingCart = new ShoppingCart { GuestId = "guest123" };

			_cartRepoMock.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<ShoppingCart, bool>>>()))
				.ReturnsAsync(existingCart);

			await _service.EnsureGuestCartExistsAsync("guest123");

			_cartRepoMock.Verify(x => x.AddAsync(It.IsAny<ShoppingCart>()), Times.Never);
		}

		[Test]
		public async Task EnsureGuestCartExistsAsync_CartDoesNotExist_AddsNewCart()
		{
			_cartRepoMock.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<ShoppingCart, bool>>>()))
				.ReturnsAsync((ShoppingCart?)null);
			_cartRepoMock.Setup(x => x.AddAsync(It.IsAny<ShoppingCart>())).Returns(Task.CompletedTask);

			await _service.EnsureGuestCartExistsAsync("guest123");

			_cartRepoMock.Verify(x => x.AddAsync(It.Is<ShoppingCart>(sc => sc.GuestId == "guest123")), Times.Once);
		}

		#endregion

		#region AddNewShoppingCartAsync Tests

		[Test]
		public async Task AddNewShoppingCartAsync_NullUser_ReturnsNull()
		{
			var result = await _service.AddNewShoppingCartAsync(null);
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task AddNewShoppingCartAsync_ValidUser_AddsCartAndReturnsIt()
		{
			var user = new ApplicationUser { Id = "user123" };
			_cartRepoMock.Setup(x => x.AddAsync(It.IsAny<ShoppingCart>())).Returns(Task.CompletedTask);

			var result = await _service.AddNewShoppingCartAsync(user);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.UserId, Is.EqualTo("user123"));
			Assert.That(result.User, Is.EqualTo(user));
			_cartRepoMock.Verify(x => x.AddAsync(It.Is<ShoppingCart>(sc => sc.UserId == "user123")), Times.Once);
		}

		#endregion
	}
}
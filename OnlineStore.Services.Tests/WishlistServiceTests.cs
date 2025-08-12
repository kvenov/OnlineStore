using Microsoft.AspNetCore.Identity;
using MockQueryable;
using Moq;
using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Services.Core;
using OnlineStore.Services.Core.Interfaces;

namespace OnlineStore.Services.Tests
{
	[TestFixture]
	public class WishlistServiceTests
	{
		private Mock<IProductRepository> _mockProductRepo;
		private Mock<IWishlistRepository> _mockWishlistRepo;
		private Mock<UserManager<ApplicationUser>> _mockUserManager;
		private IWishlistService _service;

		[SetUp]
		public void Setup()
		{
			_mockProductRepo = new Mock<IProductRepository>();
			_mockWishlistRepo = new Mock<IWishlistRepository>();

			var store = new Mock<IUserStore<ApplicationUser>>();
			_mockUserManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

			_service = new WishlistService(_mockUserManager.Object, _mockWishlistRepo.Object, _mockProductRepo.Object);
		}

		[Test]
		public async Task AddNewWishlistAsync_WithUser_ReturnsNewWishlist()
		{
			// Arrange
			var user = new ApplicationUser { Id = "userId" };

			_mockWishlistRepo.Setup(r => r.AddAsync(It.IsAny<Wishlist>()))
							 .Returns(Task.CompletedTask)
							 .Verifiable();

			// Act
			var result = await _service.AddNewWishlistAsync(user);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.UserId, Is.EqualTo(user.Id));
			_mockWishlistRepo.Verify(r => r.AddAsync(It.IsAny<Wishlist>()), Times.Once);
		}

		[Test]
		public async Task AddNewWishlistAsync_WithNullUser_ReturnsNull()
		{
			// Act
			var result = await _service.AddNewWishlistAsync(null);

			// Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task AddProductToWishlist_WithValidData_AddsProduct()
		{
			// Arrange
			var userId = "userId";
			var productId = 1;

			var product = new Product { Id = productId };
			var user = new ApplicationUser { Id = userId };
			var wishlist = new Wishlist
			{
				Id = 1,
				UserId = userId,
				WishlistItems = new List<WishlistItem>()
			};

			_mockProductRepo.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
			_mockUserManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);
			_mockWishlistRepo.Setup(r => r.GetAllAttached()).Returns(new List<Wishlist> { wishlist }.BuildMock());
			_mockWishlistRepo.Setup(r => r.AddWishlistItemAsync(It.IsAny<WishlistItem>())).Returns(Task.CompletedTask);

			// Act
			var result = await _service.AddProductToWishlist(productId, userId);

			// Assert
			Assert.That(result, Is.True);
			_mockWishlistRepo.Verify(r => r.AddWishlistItemAsync(It.IsAny<WishlistItem>()), Times.Once);
		}

		[Test]
		public async Task AddProductToWishlist_WithAlreadyAddedProduct_ReturnsFalse()
		{
			// Arrange
			var userId = "userId";
			var productId = 1;

			var product = new Product { Id = productId };
			var user = new ApplicationUser { Id = userId };
			var existingWishlistItem = new WishlistItem { ProductId = productId };
			var wishlist = new Wishlist
			{
				Id = 1,
				UserId = userId,
				WishlistItems = new List<WishlistItem> { existingWishlistItem }
			};

			_mockProductRepo.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
			_mockUserManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);
			_mockWishlistRepo.Setup(r => r.GetAllAttached()).Returns(new List<Wishlist> { wishlist }.BuildMock());

			// Act
			var result = await _service.AddProductToWishlist(productId, userId);

			// Assert
			Assert.That(result, Is.False);
			_mockWishlistRepo.Verify(r => r.AddWishlistItemAsync(It.IsAny<WishlistItem>()), Times.Never);
		}

		[Test]
		public async Task EditNoteAsync_WithValidNote_EditsSuccessfully()
		{
			// Arrange
			var itemId = 1;
			var note = "New note";
			var userId = "userId";

			var user = new ApplicationUser { Id = userId };
			var wishlistItem = new WishlistItem { Id = itemId, Notes = "Old note" };
			var wishlist = new Wishlist
			{
				UserId = userId,
				WishlistItems = new List<WishlistItem> { wishlistItem }
			};

			_mockUserManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);
			_mockWishlistRepo.Setup(r => r.GetAllAttached()).Returns(new List<Wishlist> { wishlist }.BuildMock());
			_mockWishlistRepo.Setup(r => r.GetWishlistItemByIdAsync(itemId)).ReturnsAsync(wishlistItem);
			_mockWishlistRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

			// Act
			var result = await _service.EditNoteAsync(itemId, note, userId);

			// Assert
			Assert.That(result, Is.True);
			Assert.That(wishlistItem.Notes, Is.EqualTo(note));
			_mockWishlistRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
		}

		[Test]
		public async Task EditNoteAsync_WithInvalidNote_ReturnsFalse()
		{
			// Arrange
			var itemId = 1;
			string? note = null;
			var userId = "userId";

			// Act
			var result = await _service.EditNoteAsync(itemId, note, userId);

			// Assert
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task GetUserWishlist_ReturnsWishlistViewModel()
		{
			// Arrange
			var userId = "userId";
			var wishlistId = 1;

			var wishlist = new Wishlist
			{
				Id = wishlistId,
				UserId = userId,
				WishlistItems = new List<WishlistItem>
				{
					new WishlistItem
					{
						Id = 1,
						Notes = "Note1",
						Product = new Product
						{
							Name = "Product1",
							Category = new ProductCategory { Name = "Category1" },
							ImageUrl = "image1.jpg",
							Price = 10
						}
					}
				}
			};

			var queryableWishlist = new List<Wishlist> { wishlist }.BuildMock();

			_mockWishlistRepo.Setup(r => r.GetAllAttached()).Returns(queryableWishlist);

			// Act
			var result = await _service.GetUserWishlist(userId);

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Id, Is.EqualTo(wishlistId));
			Assert.That(result.UserId, Is.EqualTo(userId));
			Assert.That(result.Items.Count(), Is.EqualTo(1));
			Assert.That(result.Items.First().ProductName, Is.EqualTo("Product1"));
		}

		[Test]
		public async Task GetWishlistItemsCountAsync_WithUser_ReturnsCount()
		{
			// Arrange
			var userId = "userId";
			var wishlist = new Wishlist
			{
				UserId = userId,
				WishlistItems = new List<WishlistItem> { new WishlistItem(), new WishlistItem() }
			};

			_mockUserManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(new ApplicationUser { Id = userId });
			_mockWishlistRepo.Setup(r => r.GetAllAttached()).Returns(new List<Wishlist> { wishlist }.BuildMock());

			// Act
			var count = await _service.GetWishlistItemsCountAsync(userId);

			// Assert
			Assert.That(count, Is.EqualTo(2));
		}

		[Test]
		public async Task RemoveProductFromWishlistAsycn_RemovesItemSuccessfully()
		{
			// Arrange
			int itemId = 1;
			var userId = "userId";

			var user = new ApplicationUser { Id = userId };
			var wishlistItem = new WishlistItem { Id = itemId };
			var wishlist = new Wishlist
			{
				UserId = userId,
				WishlistItems = new List<WishlistItem> { wishlistItem }
			};

			_mockUserManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);
			_mockWishlistRepo.Setup(r => r.GetAllAttached()).Returns(new List<Wishlist> { wishlist }.BuildMock());
			_mockWishlistRepo.Setup(r => r.GetAllWishlistItemsAttached()).Returns(new List<WishlistItem> { wishlistItem }.BuildMock());
			_mockWishlistRepo.Setup(r => r.DeleteWishlistItem(wishlistItem));
			_mockWishlistRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

			// Act
			var result = await _service.RemoveProductFromWishlistAsycn(itemId, userId);

			// Assert
			Assert.That(result, Is.True);
			_mockWishlistRepo.Verify(r => r.DeleteWishlistItem(wishlistItem), Times.Once);
			_mockWishlistRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
		}

		[Test]
		public async Task RemoveProductFromWishlistAsycn_WithInvalidItemId_ReturnsFalse()
		{
			// Arrange
			int? itemId = null;
			var userId = "userId";

			// Act
			var result = await _service.RemoveProductFromWishlistAsycn(itemId, userId);

			// Assert
			Assert.That(result, Is.False);
		}
	}
}
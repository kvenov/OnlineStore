using Microsoft.AspNetCore.Identity;
using MockQueryable;
using Moq;
using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Services.Core;
using OnlineStore.Services.Core.Interfaces;
using System.Reflection;

namespace OnlineStore.Services.Tests
{
	[TestFixture]
	public class ProductServiceTests
	{
		private Mock<IProductRepository> _mockProductRepo;
		private Mock<IRepository<ProductCategory, int>> _mockCategoryRepo;
		private Mock<UserManager<ApplicationUser>> _mockUserManager;
		private IProductService _service;

		[SetUp]
		public void SetUp()
		{
			_mockProductRepo = new Mock<IProductRepository>();
			_mockCategoryRepo = new Mock<IRepository<ProductCategory, int>>();
			_mockUserManager = new Mock<UserManager<ApplicationUser>>(
				Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

			_service = new ProductService(
				_mockProductRepo.Object,
				_mockCategoryRepo.Object,
				_mockUserManager.Object
			);
		}

		[Test]
		public async Task GetAllProductsAsync_ReturnsMappedProducts()
		{
			var products = new List<Product>
			{
				new Product
				{
					Id = 1,
					Name = "Test1",
					Description = "Desc1",
					Price = 10.5m,
					ImageUrl = "img1",
					AverageRating = 4.5
				},
				new Product
				{
					Id = 2,
					Name = "Test2",
					Description = "Desc2",
					Price = 20.0m,
					ImageUrl = "img2",
					AverageRating = 3.0
				}
			}.BuildMock();

			_mockProductRepo.Setup(r => r.GetAllAttached()).Returns(products);

			var result = await _service.GetAllProductsAsync();

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count(), Is.EqualTo(2));
			Assert.That(result.First().Id, Is.EqualTo(1));
			Assert.That(result.First().Name, Is.EqualTo("Test1"));
			Assert.That(result.First().Rating, Is.EqualTo(4.5f));
			Assert.That(result.Last().Id, Is.EqualTo(2));
			Assert.That(result.Last().Name, Is.EqualTo("Test2"));
			Assert.That(result.Last().Rating, Is.EqualTo(3.0f));
		}

		[Test]
		public async Task GetAllProductsAsync_ReturnsEmpty_WhenNoProducts()
		{
			var products = new List<Product>().BuildMock();
			_mockProductRepo.Setup(r => r.GetAllAttached()).Returns(products);

			var result = await _service.GetAllProductsAsync();

			Assert.That(result, Is.Empty);
		}

		[Test]
		public async Task GetProductByIdAsync_ReturnsNull_WhenIdIsNull()
		{
			var result = await _service.GetProductByIdAsync(null);
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetProductByIdAsync_ReturnsProduct_WhenFound()
		{
			var products = new List<Product>
			{
				new Product
				{
					Id = 5,
					Name = "TestProduct",
					Description = "Desc",
					Price = 99.99m,
					ImageUrl = "img"
				}
			}.BuildMock();

			_mockProductRepo.Setup(r => r.GetAllAttached()).Returns(products);

			var result = await _service.GetProductByIdAsync(5);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Id, Is.EqualTo(5));
			Assert.That(result.Name, Is.EqualTo("TestProduct"));
			Assert.That(result.Description, Is.EqualTo("Desc"));
			Assert.That(result.Price, Is.EqualTo(99.99m));
			Assert.That(result.ImageUrl, Is.EqualTo("img"));
		}

		[Test]
		public void GetProductByIdAsync_Throws_WhenProductNotFound()
		{
			var products = new List<Product>().BuildMock();
			_mockProductRepo.Setup(r => r.GetAllAttached()).Returns(products);

			var ex = Assert.ThrowsAsync<TargetInvocationException>(async () => await _service.GetProductByIdAsync(42));
			Assert.That(ex.InnerException, Is.TypeOf<InvalidOperationException>());
		}

		[Test]
		public async Task GetProductDetailsByIdAsync_ReturnsNull_WhenIdIsNull()
		{
			var result = await _service.GetProductDetailsByIdAsync(null, "user1");
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetProductDetailsByIdAsync_ReturnsNull_WhenProductNotFound()
		{
			var products = new List<Product>().BuildMock();
			_mockProductRepo.Setup(r => r.GetAllAttached()).Returns(products);

			var result = await _service.GetProductDetailsByIdAsync(1, "user1");
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetProductDetailsByIdAsync_ReturnsMappedViewModel_WhenProductFound()
		{
			var product = new Product
			{
				Id = 1,
				Name = "Test",
				Description = "Desc",
				Price = 99.99m,
				DiscountPrice = 79.99m,
				ImageUrl = "img",
				AverageRating = 4.5,
				Category = new ProductCategory { Name = "Sporty Shoes" },
				Brand = new Brand { Name = "Nike" },
				ProductDetails = new ProductDetails
				{
					Material = "Leather",
					Color = "Black",
					Gender = "Unisex",
					SizeGuideUrl = "url",
					CountryOfOrigin = "USA",
					CareInstructions = "Care",
					Weight = 1.2m,
					Fit = "Regular",
					Style = "Sport"
				},
				ProductReviews = new List<ProductReview>
				{
					new ProductReview { Id = 10, UserId = "user1", User = new ApplicationUser { UserName = "User1" }, Content = "Great!", IsDeleted = false },
					new ProductReview { Id = 11, UserId = "user2", User = new ApplicationUser { UserName = "User2" }, Content = "Nice!", IsDeleted = false }
				},
				ProductRatings = new List<ProductRating>
				{
					new ProductRating { Id = 20, UserId = "user1", Rating = 5, IsDeleted = false },
					new ProductRating { Id = 21, UserId = "user2", Rating = 4, IsDeleted = false }
				}
			};
			var products = new List<Product> { product }.BuildMock();
			_mockProductRepo.Setup(r => r.GetAllAttached()).Returns(products);

			var result = await _service.GetProductDetailsByIdAsync(1, "user1");

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Id, Is.EqualTo(1));
			Assert.That(result.Name, Is.EqualTo("Test"));
			Assert.That(result.Brand, Is.EqualTo("Nike"));
			Assert.That(result.Category, Is.EqualTo("Sporty Shoes"));
			Assert.That(result.Details.Material, Is.EqualTo("Leather"));
			Assert.That(result.Details.Fit, Is.EqualTo("Regular"));
			Assert.That(result.Reviews.Count(), Is.EqualTo(2));
			Assert.That(result.Ratings.Count(), Is.EqualTo(2));
			Assert.That(result.IsProductReviewed, Is.True);
			Assert.That(result.AvailableSizes.Contains("36"));
		}

		[Test]
		public async Task AddProductReviewAsync_ReturnsFalse_WhenAnyArgumentIsNull()
		{
			var result1 = await _service.AddProductReviewAsync(null, 5, "content", "user1");
			var result2 = await _service.AddProductReviewAsync(1, null, "content", "user1");
			var result3 = await _service.AddProductReviewAsync(1, 5, null, "user1");
			Assert.That(result1, Is.False);
			Assert.That(result2, Is.False);
			Assert.That(result3, Is.False);
		}

		[Test]
		public async Task AddProductReviewAsync_ReturnsFalse_WhenProductOrUserIsNull()
		{
			_mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product)null);
			_mockUserManager.Setup(u => u.FindByIdAsync("user1")).ReturnsAsync((ApplicationUser)null);

			_mockProductRepo.Setup(r => r.GetAllReviewsAttached()).Returns(new List<ProductReview>().BuildMock());
			_mockProductRepo.Setup(r => r.GetAllRatingsAttached()).Returns(new List<ProductRating>().BuildMock());

			var result = await _service.AddProductReviewAsync(1, 5, "content", "user1");
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task AddProductReviewAsync_ReturnsFalse_WhenRatingOrContentInvalid()
		{
			var product = new Product { Id = 1 };
			var user = new ApplicationUser { Id = "user1" };
			_mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
			_mockUserManager.Setup(u => u.FindByIdAsync("user1")).ReturnsAsync(user);

			_mockProductRepo.Setup(r => r.GetAllReviewsAttached()).Returns(new List<ProductReview>().BuildMock());
			_mockProductRepo.Setup(r => r.GetAllRatingsAttached()).Returns(new List<ProductRating>().BuildMock());

			var result1 = await _service.AddProductReviewAsync(1, 0, "content", "user1");
			var result2 = await _service.AddProductReviewAsync(1, 6, "content", "user1");
			var result3 = await _service.AddProductReviewAsync(1, 5, "   ", "user1");
			Assert.That(result1, Is.False);
			Assert.That(result2, Is.False);
			Assert.That(result3, Is.False);
		}

		[Test]
		public async Task AddProductReviewAsync_AddsNewReviewAndRating_WhenNoneExist()
		{
			var product = new Product { Id = 1 };
			var user = new ApplicationUser { Id = "user1" };
			_mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
			_mockUserManager.Setup(u => u.FindByIdAsync("user1")).ReturnsAsync(user);
			_mockProductRepo.Setup(r => r.GetAllReviewsAttached()).Returns(new List<ProductReview>().BuildMock());
			_mockProductRepo.Setup(r => r.GetAllRatingsAttached()).Returns(new List<ProductRating>().BuildMock());
			_mockProductRepo.Setup(r => r.AddProductReviewAsync(It.IsAny<ProductReview>())).Returns(Task.CompletedTask).Verifiable();
			_mockProductRepo.Setup(r => r.AddProductRatingAsync(It.IsAny<ProductRating>())).Returns(Task.CompletedTask).Verifiable();
			_mockProductRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();

			var result = await _service.AddProductReviewAsync(1, 5, "content", "user1");

			Assert.That(result, Is.True);
			_mockProductRepo.Verify(r => r.AddProductReviewAsync(It.IsAny<ProductReview>()), Times.Once);
			_mockProductRepo.Verify(r => r.AddProductRatingAsync(It.IsAny<ProductRating>()), Times.Once);
			_mockProductRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
		}

		[Test]
		public async Task AddProductReviewAsync_UpdatesExistingReviewAndRating_WhenExist()
		{
			var product = new Product { Id = 1 };
			var user = new ApplicationUser { Id = "user1" };
			var review = new ProductReview { ProductId = 1, UserId = "user1", Content = "old", IsDeleted = true };
			var rating = new ProductRating { ProductId = 1, UserId = "user1", Rating = 1, IsDeleted = true };
			var reviews = new List<ProductReview> { review }.BuildMock();
			var ratings = new List<ProductRating> { rating }.BuildMock();

			_mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
			_mockUserManager.Setup(u => u.FindByIdAsync("user1")).ReturnsAsync(user);
			_mockProductRepo.Setup(r => r.GetAllReviewsAttached()).Returns(reviews);
			_mockProductRepo.Setup(r => r.GetAllRatingsAttached()).Returns(ratings);
			_mockProductRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();

			var result = await _service.AddProductReviewAsync(1, 5, "new content", "user1");

			Assert.That(result, Is.True);
			Assert.That(review.IsDeleted, Is.False);
			Assert.That(review.Content, Is.EqualTo("new content"));
			Assert.That(rating.IsDeleted, Is.False);
			Assert.That(rating.Rating, Is.EqualTo(5));
			_mockProductRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
		}

		[Test]
		public async Task EditProductReviewAsync_ReturnsFalse_WhenAnyArgumentIsNull()
		{
			var result1 = await _service.EditProductReviewAsync(null, 5, 1, "content", "user1");
			var result2 = await _service.EditProductReviewAsync(1, null, 1, "content", "user1");
			var result3 = await _service.EditProductReviewAsync(1, 5, null, "content", "user1");
			var result4 = await _service.EditProductReviewAsync(1, 5, 1, null, "user1");
			Assert.That(result1, Is.False);
			Assert.That(result2, Is.False);
			Assert.That(result3, Is.False);
			Assert.That(result4, Is.False);
		}

		[Test]
		public async Task EditProductReviewAsync_ReturnsFalse_WhenReviewOrRatingOrUserIsNull()
		{
			_mockProductRepo.Setup(r => r.GetProductReviewByIdAsync(1)).ReturnsAsync((ProductReview)null);
			_mockProductRepo.Setup(r => r.GetProductRatingByIdAsync(1)).ReturnsAsync((ProductRating)null);
			_mockUserManager.Setup(u => u.FindByIdAsync("user1")).ReturnsAsync((ApplicationUser)null);

			var result = await _service.EditProductReviewAsync(1, 5, 1, "content", "user1");
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task EditProductReviewAsync_ReturnsFalse_WhenUserIdMismatchOrRatingInvalid()
		{
			var review = new ProductReview { UserId = "other" };
			var rating = new ProductRating { UserId = "other" };
			var user = new ApplicationUser { Id = "user1" };
			_mockProductRepo.Setup(r => r.GetProductReviewByIdAsync(1)).ReturnsAsync(review);
			_mockProductRepo.Setup(r => r.GetProductRatingByIdAsync(1)).ReturnsAsync(rating);
			_mockUserManager.Setup(u => u.FindByIdAsync("user1")).ReturnsAsync(user);

			var result1 = await _service.EditProductReviewAsync(1, 0, 1, "content", "user1");
			var result2 = await _service.EditProductReviewAsync(1, 6, 1, "content", "user1");
			var result3 = await _service.EditProductReviewAsync(1, 5, 1, "content", "user1");
			Assert.That(result1, Is.False);
			Assert.That(result2, Is.False);
			Assert.That(result3, Is.False);
		}

		[Test]
		public async Task EditProductReviewAsync_UpdatesReviewAndRating_WhenValid()
		{
			var review = new ProductReview { UserId = "user1", Content = "old" };
			var rating = new ProductRating { UserId = "user1", Rating = 1 };
			var user = new ApplicationUser { Id = "user1" };
			_mockProductRepo.Setup(r => r.GetProductReviewByIdAsync(1)).ReturnsAsync(review);
			_mockProductRepo.Setup(r => r.GetProductRatingByIdAsync(2)).ReturnsAsync(rating);
			_mockUserManager.Setup(u => u.FindByIdAsync("user1")).ReturnsAsync(user);
			_mockProductRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();

			var result = await _service.EditProductReviewAsync(1, 5, 2, "new content", "user1");

			Assert.That(result, Is.True);
			Assert.That(review.Content, Is.EqualTo("new content"));
			Assert.That(rating.Rating, Is.EqualTo(5));
			_mockProductRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
		}

		[Test]
		public async Task RemoveProductReviewAsync_ReturnsFalse_WhenReviewIdOrRatingIdIsNull()
		{
			var result1 = await _service.RemoveProductReviewAsync(null, 1, "user1");
			var result2 = await _service.RemoveProductReviewAsync(1, null, "user1");
			Assert.That(result1, Is.False);
			Assert.That(result2, Is.False);
		}

		[Test]
		public async Task RemoveProductReviewAsync_ReturnsFalse_WhenReviewOrRatingOrUserIsNull()
		{
			_mockProductRepo.Setup(r => r.GetProductReviewByIdAsync(1)).ReturnsAsync((ProductReview)null);
			_mockProductRepo.Setup(r => r.GetProductRatingByIdAsync(1)).ReturnsAsync((ProductRating)null);
			_mockUserManager.Setup(u => u.FindByIdAsync("user1")).ReturnsAsync((ApplicationUser)null);

			var result = await _service.RemoveProductReviewAsync(1, 1, "user1");
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task RemoveProductReviewAsync_ReturnsFalse_WhenUserIdMismatch()
		{
			var review = new ProductReview { UserId = "other" };
			var rating = new ProductRating { UserId = "other" };
			var user = new ApplicationUser { Id = "user1" };
			_mockProductRepo.Setup(r => r.GetProductReviewByIdAsync(1)).ReturnsAsync(review);
			_mockProductRepo.Setup(r => r.GetProductRatingByIdAsync(1)).ReturnsAsync(rating);
			_mockUserManager.Setup(u => u.FindByIdAsync("user1")).ReturnsAsync(user);

			var result = await _service.RemoveProductReviewAsync(1, 1, "user1");
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task RemoveProductReviewAsync_ReturnsTrue_WhenReviewAndRatingRemoved()
		{
			var review = new ProductReview { UserId = "user1" };
			var rating = new ProductRating { UserId = "user1" };
			var user = new ApplicationUser { Id = "user1" };
			_mockProductRepo.Setup(r => r.GetProductReviewByIdAsync(1)).ReturnsAsync(review);
			_mockProductRepo.Setup(r => r.GetProductRatingByIdAsync(2)).ReturnsAsync(rating);
			_mockUserManager.Setup(u => u.FindByIdAsync("user1")).ReturnsAsync(user);
			_mockProductRepo.Setup(r => r.DeleteReviewAsync(review)).ReturnsAsync(true).Verifiable();
			_mockProductRepo.Setup(r => r.DeleteRatingAsync(rating)).ReturnsAsync(true).Verifiable();

			var result = await _service.RemoveProductReviewAsync(1, 2, "user1");

			Assert.That(result, Is.True);
			_mockProductRepo.Verify(r => r.DeleteReviewAsync(review), Times.Once);
			_mockProductRepo.Verify(r => r.DeleteRatingAsync(rating), Times.Once);
		}

		[Test]
		public async Task RemoveProductReviewAsync_ReturnsFalse_WhenEitherDeleteFails()
		{
			var review = new ProductReview { UserId = "user1" };
			var rating = new ProductRating { UserId = "user1" };
			var user = new ApplicationUser { Id = "user1" };
			_mockProductRepo.Setup(r => r.GetProductReviewByIdAsync(1)).ReturnsAsync(review);
			_mockProductRepo.Setup(r => r.GetProductRatingByIdAsync(2)).ReturnsAsync(rating);
			_mockUserManager.Setup(u => u.FindByIdAsync("user1")).ReturnsAsync(user);
			_mockProductRepo.Setup(r => r.DeleteReviewAsync(review)).ReturnsAsync(true);
			_mockProductRepo.Setup(r => r.DeleteRatingAsync(rating)).ReturnsAsync(false);

			var result = await _service.RemoveProductReviewAsync(1, 2, "user1");
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task GetBestProductsAsync_ReturnsTrendingProducts()
		{
			var products = new List<Product>
			{
				new Product
				{
					Id = 1,
					Name = "A",
					ImageUrl = "imgA",
					Price = 10.5m,
					IsActive = true,
					StockQuantity = 5,
					AverageRating = 4.5,
					OrderItems = new List<OrderItem> { new OrderItem { Quantity = 2 }, new OrderItem { Quantity = 3 } }
				},
				new Product
				{
					Id = 2,
					Name = "B",
					ImageUrl = "imgB",
					Price = 20.0m,
					IsActive = true,
					StockQuantity = 10,
					AverageRating = 3.0,
					OrderItems = new List<OrderItem> { new OrderItem { Quantity = 1 } }
				}
			}.BuildMock();

			_mockProductRepo.Setup(r => r.GetAllAttached()).Returns(products);

			var result = await _service.GetBestProductsAsync();

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count(), Is.EqualTo(2));
			Assert.That(result.First().ProductId, Is.EqualTo(1));
			Assert.That(result.First().ProductName, Is.EqualTo("A"));
			Assert.That(result.First().ImageUrl, Is.EqualTo("imgA"));
			Assert.That(result.First().Price, Is.EqualTo("10.50"));
		}

		[Test]
		public async Task GetBestProductsAsync_ReturnsEmpty_WhenNoProducts()
		{
			var products = new List<Product>().BuildMock();
			_mockProductRepo.Setup(r => r.GetAllAttached()).Returns(products);

			var result = await _service.GetBestProductsAsync();

			Assert.That(result, Is.Empty);
		}

		[Test]
		public async Task GetFilteredProductsAsync_ReturnsEmpty_WhenAnyArgumentIsNullOrWhitespace()
		{
			var result1 = await _service.GetFilteredProductsAsync(null, "cat", "sub");
			var result2 = await _service.GetFilteredProductsAsync("gender", null, "sub");
			var result3 = await _service.GetFilteredProductsAsync("gender", "cat", null);
			var result4 = await _service.GetFilteredProductsAsync("", "cat", "sub");
			Assert.That(result1, Is.Empty);
			Assert.That(result2, Is.Empty);
			Assert.That(result3, Is.Empty);
			Assert.That(result4, Is.Empty);
		}

		[Test]
		public async Task GetFilteredProductsAsync_ReturnsEmpty_WhenGenderOrCategoryOrSubCategoryInvalid()
		{
			_mockCategoryRepo.Setup(r => r.GetAllAttached()).Returns(new List<ProductCategory>().BuildMock());

			var result = await _service.GetFilteredProductsAsync("male", "cat", "sub");
			Assert.That(result, Is.Empty);
		}

		[Test]
		public async Task GetFilteredProductsAsync_ReturnsFilteredProducts_WhenValid()
		{
			var allowedGenders = new[] { "male", "female", "unisex" };
			typeof(ProductService)
				.GetField("AllowedGenders", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
				?.SetValue(null, allowedGenders);

			var parentCategory = new ProductCategory { Name = "cat", ParentCategoryId = null };
			var subCategory = new ProductCategory { Name = "sub", ParentCategoryId = 1, ParentCategory = parentCategory };
			var categories = new List<ProductCategory> { parentCategory, subCategory }.BuildMock();

			_mockCategoryRepo.Setup(r => r.GetAllAttached()).Returns(categories);

			var products = new List<Product>
			{
				new Product
				{
					Id = 1,
					Name = "Filtered",
					Description = "Desc",
					Price = 10.5m,
					ImageUrl = "img",
					AverageRating = 4.5,
					Category = subCategory,
					ProductDetails = new ProductDetails { Gender = "male" }
				}
			}.BuildMock();

			_mockProductRepo.Setup(r => r.GetAllAttached()).Returns(products);

			var result = await _service.GetFilteredProductsAsync("male", "cat", "sub");

			Assert.That(result, Is.Not.Empty);
			Assert.That(result.First().Id, Is.EqualTo(1));
			Assert.That(result.First().Name, Is.EqualTo("Filtered"));
		}

		[Test]
		public async Task GetSearchedProductsAsync_ReturnsEmpty_WhenQueryIsNullOrWhitespace()
		{
			var result1 = await _service.GetSearchedProductsAsync(null);
			var result2 = await _service.GetSearchedProductsAsync("");
			var result3 = await _service.GetSearchedProductsAsync("   ");
			Assert.That(result1, Is.Empty);
			Assert.That(result2, Is.Empty);
			Assert.That(result3, Is.Empty);
		}

		[Test]
		public async Task GetSearchedProductsAsync_ReturnsMatchingProducts()
		{
			var products = new List<Product>
			{
				new Product
				{
					Id = 1,
					Name = "Shoe",
					Description = "Sporty",
					Price = 99.99m,
					ImageUrl = "img"
				},
				new Product
				{
					Id = 2,
					Name = "Shirt",
					Description = "Casual",
					Price = 49.99m,
					ImageUrl = "img2"
				}
			}.BuildMock();

			_mockProductRepo.Setup(r => r.GetAllAttached()).Returns(products);

			var result = await _service.GetSearchedProductsAsync("shoe");

			Assert.That(result.Count(), Is.EqualTo(1));
			Assert.That(result.First().Id, Is.EqualTo(1));
			Assert.That(result.First().Name, Is.EqualTo("Shoe"));
			Assert.That(result.First().Price, Is.EqualTo("£99.99").Or.EqualTo("$99.99").Or.EqualTo("99.99 €"));
		}

		[Test]
		public async Task GetAllProductsAsync_ReturnsEmptyViewModel_WhenQueryIsNullOrWhitespace()
		{
			var result1 = await _service.GetAllProductsAsync(null);
			var result2 = await _service.GetAllProductsAsync("");
			var result3 = await _service.GetAllProductsAsync("   ");
			Assert.That(result1.Products, Is.Empty);
			Assert.That(result1.SubCategories, Is.Empty);
			Assert.That(result2.Products, Is.Empty);
			Assert.That(result2.SubCategories, Is.Empty);
			Assert.That(result3.Products, Is.Empty);
			Assert.That(result3.SubCategories, Is.Empty);
		}

		[Test]
		public async Task GetAllProductsAsync_ReturnsProductsAndSubCategories_WhenQueryMatches()
		{
			var parentCategory = new ProductCategory
			{
				Name = "Shoes",
				Subcategories = new List<ProductCategory>
				{
					new ProductCategory { Name = "Sporty" },
					new ProductCategory { Name = "Lifestyle" }
				}
			};
			var product = new Product
			{
				Id = 1,
				Name = "Sporty Shoes",
				Description = "Best for running",
				Price = 99.99m,
				ImageUrl = "img",
				AverageRating = 4.5,
				Category = new ProductCategory { Name = "Sporty", ParentCategory = parentCategory }
			};
			var products = new List<Product> { product }.BuildMock();
			_mockProductRepo.Setup(r => r.GetAllAttached()).Returns(products);

			// Mock parentCategory selection
			var parentCategories = new List<ProductCategory> { parentCategory }.BuildMock();
			_mockProductRepo.Setup(r => r.GetAllAttached()).Returns(products);

			var result = await _service.GetAllProductsAsync("sporty");

			Assert.That(result.Products.Count(), Is.EqualTo(1));
			Assert.That(result.Products.First().Name, Is.EqualTo("Sporty Shoes"));
			Assert.That(result.SubCategories, Is.Not.Null);
			Assert.That(result.SubCategories.Count(), Is.EqualTo(2));
			Assert.That(result.SubCategories, Does.Contain("Sporty"));
			Assert.That(result.SubCategories, Does.Contain("Lifestyle"));
		}

		[Test]
		public async Task GetProductSizesAsync_ReturnsNull_WhenProductIdIsNull()
		{
			var result = await _service.GetProductSizesAsync(null);
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetProductSizesAsync_ReturnsNull_WhenProductNotFound()
		{
			_mockProductRepo.Setup(r => r.SingleOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Product, bool>>>())).ReturnsAsync((Product)null);

			var result = await _service.GetProductSizesAsync(1);
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetProductSizesAsync_ReturnsSizes_WhenProductFound()
		{
			var category = new ProductCategory { Name = "Sporty Shoes" };
			var product = new Product { Id = 1, Category = category };
			_mockProductRepo.Setup(r => r.SingleOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Product, bool>>>())).ReturnsAsync(product);

			var result = await _service.GetProductSizesAsync(1);

			Assert.That(result, Is.Not.Null);
			Assert.That(result, Does.Contain("36"));
			Assert.That(result, Does.Contain("47"));
			Assert.That(result.Count(), Is.EqualTo(12));
		}

		[Test]
		public async Task GetProductSizesAsync_ReturnsOneSize_WhenUnknownCategory()
		{
			var category = new ProductCategory { Name = "Unknown" };
			var product = new Product { Id = 2, Category = category };
			_mockProductRepo.Setup(r => r.SingleOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Product, bool>>>())).ReturnsAsync(product);

			var result = await _service.GetProductSizesAsync(2);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count(), Is.EqualTo(1));
			Assert.That(result.First(), Is.EqualTo("One Size"));
		}
	}
}
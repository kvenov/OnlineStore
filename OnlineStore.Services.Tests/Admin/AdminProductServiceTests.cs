using MockQueryable;
using Moq;
using OnlineStore.Data.Common.Constants;
using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Services.Core.Admin;
using OnlineStore.Services.Core.Admin.Interfaces;
using OnlineStore.Web.ViewModels.Admin.Product;
using System.Reflection;

namespace OnlineStore.Services.Tests.Admin
{
	[TestFixture]
	public class AdminProductServiceTests
	{
		private Mock<IProductRepository> _mockProductRepo;
		private Mock<IAsyncRepository<ProductCategory, int>> _mockCategoryRepo;
		private Mock<IAsyncRepository<Brand, int>> _mockBrandRepo;
		private Mock<IRepository<ProductCategory, int>> _mockCategoryRepoSync;
		private Mock<IRepository<Brand, int>> _mockBrandRepoSync;
		private IAdminProductService _service;

		[SetUp]
		public void Setup()
		{
			_mockProductRepo = new Mock<IProductRepository>();
			_mockCategoryRepo = new Mock<IAsyncRepository<ProductCategory, int>>();
			_mockBrandRepo = new Mock<IAsyncRepository<Brand, int>>();
			_mockCategoryRepoSync = new Mock<IRepository<ProductCategory, int>>();
			_mockBrandRepoSync = new Mock<IRepository<Brand, int>>();

			_service = new AdminProductService(
				_mockProductRepo.Object,
				_mockCategoryRepo.Object,
				_mockBrandRepo.Object,
				_mockCategoryRepoSync.Object,
				_mockBrandRepoSync.Object
			);
		}

		[Test]
		public async Task AddProductAsync_ShouldReturnTrue_WhenProductIsAddedSuccessfully()
		{
			var category = new ProductCategory { Id = 1, Name = "Shoes", Products = new List<Product>() };
			var brand = new Brand { Id = 1, Name = "Nike", Products = new List<Product>() };

			var model = new AddProductInputModel
			{
				Name = "Air Max",
				CategoryId = category.Id,
				BrandId = brand.Id,
				Description = "Running shoes",
				ImageUrl = "https://example.com/airmax.jpg",
				Price = 120.0m,
				DiscountPrice = 100.0m,
				IsActive = true,
				StockQuantity = 50,
				AverageRating = "4.5",
				TotalRatings = 10,
				Material = "Leather",
				Color = "Red",
				Gender = "Male",
				SizeGuideUrl = "https://example.com/sizeguide",
				CountryOfOrigin = "USA",
				CareInstructions = "Wipe with a cloth",
				Weight = 1.2m,
				Fit = "Regular",
				Style = "Sport"
			};

			var categories = new List<ProductCategory> { category }.BuildMock();
			var brands = new List<Brand> { brand }.BuildMock();

			_mockCategoryRepoSync.Setup(r => r.GetAllAttached()).Returns(categories);
			_mockBrandRepoSync.Setup(r => r.GetAllAttached()).Returns(brands);
			_mockProductRepo.Setup(r => r.AddAsync(It.IsAny<Product>())).Returns(Task.CompletedTask).Verifiable();

			var result = await _service.AddProductAsync(model);

			Assert.That(result, Is.True);
			_mockProductRepo.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
			Assert.That(category.Products.Count, Is.EqualTo(1));
			Assert.That(brand.Products.Count, Is.EqualTo(1));
		}

		[Test]
		public async Task AddProductAsync_ShouldReturnFalse_WhenCategoryNotFound()
		{
			var brand = new Brand { Id = 1, Name = "Nike", Products = new List<Product>() };
			var model = new AddProductInputModel { CategoryId = 99, BrandId = brand.Id };

			var categories = new List<ProductCategory>().BuildMock();
			var brands = new List<Brand> { brand }.BuildMock();

			_mockCategoryRepoSync.Setup(r => r.GetAllAttached()).Returns(categories);
			_mockBrandRepoSync.Setup(r => r.GetAllAttached()).Returns(brands);

			var result = await _service.AddProductAsync(model);

			Assert.That(result, Is.False);
			_mockProductRepo.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
		}

		[Test]
		public async Task AddProductAsync_ShouldReturnFalse_WhenBrandNotFound()
		{
			var category = new ProductCategory { Id = 1, Name = "Shoes", Products = new List<Product>() };
			var model = new AddProductInputModel { CategoryId = category.Id, BrandId = 99 };

			var categories = new List<ProductCategory> { category }.BuildMock();
			var brands = new List<Brand>().BuildMock();

			_mockCategoryRepoSync.Setup(r => r.GetAllAttached()).Returns(categories);
			_mockBrandRepoSync.Setup(r => r.GetAllAttached()).Returns(brands);

			var result = await _service.AddProductAsync(model);

			Assert.That(result, Is.False);
			_mockProductRepo.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
		}

		[Test]
		public void AddProductAsync_ShouldThrow_WhenProductRepositoryThrows()
		{
			var category = new ProductCategory { Id = 1, Name = "Shoes", Products = new List<Product>() };
			var brand = new Brand { Id = 1, Name = "Nike", Products = new List<Product>() };
			var model = new AddProductInputModel
			{
				CategoryId = category.Id,
				BrandId = brand.Id,
				Name = "Test Product",
				Description = "Test Description",
				ImageUrl = "http://test.com/image.jpg",
				Price = 10.0m,
				DiscountPrice = 5.0m,
				IsActive = true,
				StockQuantity = 100,
				AverageRating = "4.5",
				TotalRatings = 10,
				Material = "Leather",
				Color = "Black",
				Gender = "Unisex",
				SizeGuideUrl = "http://test.com/size",
				CountryOfOrigin = "USA",
				CareInstructions = "Wipe clean",
				Weight = 1.0m,
				Fit = "Regular",
				Style = "Sport"
			};

			var categories = new List<ProductCategory> { category }.BuildMock();
			var brands = new List<Brand> { brand }.BuildMock();

			_mockCategoryRepoSync.Setup(r => r.GetAllAttached()).Returns(categories);
			_mockBrandRepoSync.Setup(r => r.GetAllAttached()).Returns(brands);
			_mockProductRepo.Setup(r => r.AddAsync(It.IsAny<Product>()))
							.ThrowsAsync(new System.Exception("Repo error"));

			Assert.That(async () => await _service.AddProductAsync(model),
						Throws.Exception.Message.EqualTo("Repo error"));
		}

		[Test]
		public async Task SoftDeleteProductAsync_ShouldReturnFalse_WhenIdIsNullOrWhitespace()
		{
			var resultNull = await _service.SoftDeleteProductAsync(null);
			var resultEmpty = await _service.SoftDeleteProductAsync("");
			var resultWhitespace = await _service.SoftDeleteProductAsync("   ");
			Assert.That(resultNull, Is.False);
			Assert.That(resultEmpty, Is.False);
			Assert.That(resultWhitespace, Is.False);
		}

		[Test]
		public async Task SoftDeleteProductAsync_ShouldReturnFalse_WhenProductNotFound()
		{
			string id = "1";
			_mockProductRepo.Setup(r => r.SingleOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Product, bool>>>()))
				.ReturnsAsync((Product)null);

			var result = await _service.SoftDeleteProductAsync(id);
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task SoftDeleteProductAsync_ShouldReturnTrue_WhenProductDeleted()
		{
			string id = "1";
			var product = new Product { Id = 1, Name = "Test", IsDeleted = false };
			_mockProductRepo.Setup(r => r.SingleOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Product, bool>>>()))
				.ReturnsAsync(product);
			_mockProductRepo.Setup(r => r.DeleteAsync(product)).ReturnsAsync(true);

			var result = await _service.SoftDeleteProductAsync(id);
			Assert.That(result, Is.True);
		}

		[Test]
		public async Task GetAllProductsAsync_ShouldReturnMappedProducts()
		{
			var products = new List<Product>
			{
				new Product
				{
					Id = 1,
					Name = "Test",
					Description = "Desc",
					Price = 10.5m,
					DiscountPrice = 5.0m,
					Category = new ProductCategory { Name = "Cat" },
					Brand = new Brand { Name = "Brand" },
					ImageUrl = "img"
				},
				new Product
				{
					Id = 2,
					Name = "Test2",
					Description = "Desc2",
					Price = 20.0m,
					DiscountPrice = null,
					Category = new ProductCategory { Name = "Cat2" },
					Brand = null,
					ImageUrl = "img2"
				}
			}.BuildMock();

			_mockProductRepo.Setup(r => r.GetAllAttached()).Returns(products);

			var result = await _service.GetAllProductsAsync();
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count(), Is.EqualTo(2));
			Assert.That(result.First().Id, Is.EqualTo("1"));
			Assert.That(result.First().Brand, Is.EqualTo("Brand"));
			Assert.That(result.Last().DiscountPrice, Is.Null);
			Assert.That(result.Last().Brand, Is.EqualTo(""));
		}

		[Test]
		public async Task GetProductDetailsForDeleteAsync_ShouldReturnNull_WhenIdIsInvalid()
		{
			var result = await _service.GetProductDetailsForDeleteAsync("notanumber");
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetProductDetailsForDeleteAsync_ShouldReturnNull_WhenProductNotFound()
		{
			var products = new List<Product>().BuildMock();
			_mockProductRepo.Setup(r => r.GetAllAttached()).Returns(products);

			var result = await _service.GetProductDetailsForDeleteAsync("1");
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetProductDetailsForDeleteAsync_ShouldReturnViewModel_WhenProductFound()
		{
			var products = new List<Product>
			{
				new Product
				{
					Id = 1,
					Name = "Test",
					Description = "Desc",
					Price = 10.5m,
					Category = new ProductCategory { Name = "Cat" },
					Brand = new Brand { Name = "Brand" },
					ImageUrl = "img"
				}
			}.BuildMock();

			_mockProductRepo.Setup(r => r.GetAllAttached()).Returns(products);

			var result = await _service.GetProductDetailsForDeleteAsync("1");
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Id, Is.EqualTo("1"));
			Assert.That(result.Name, Is.EqualTo("Test"));
			Assert.That(result.Category, Is.EqualTo("Cat"));
			Assert.That(result.Brand, Is.EqualTo("Brand"));
			Assert.That(result.ImageUrl, Is.EqualTo("img"));
			Assert.That(result.Price, Is.EqualTo("10.50"));
		}
		
		[Test]
		public async Task GetEditableProductByIdAsync_ShouldReturnNull_WhenIdIsNull()
		{
			var result = await _service.GetEditableProductByIdAsync(null);
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetEditableProductByIdAsync_ShouldReturnNull_WhenProductNotFound()
		{
			var products = new List<Product>().BuildMock();
			_mockProductRepo.Setup(r => r.GetAllAttached()).Returns(products);

			var result = await _service.GetEditableProductByIdAsync(1);
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetEditableProductByIdAsync_ShouldReturnEditProductInputModel_WhenProductFound()
		{
			var product = new Product
			{
				Id = 1,
				Name = "Test",
				Description = "Desc",
				ImageUrl = "img",
				Price = 10.5m,
				DiscountPrice = 5.0m,
				IsActive = true,
				StockQuantity = 100,
				AverageRating = 4.5,
				TotalRatings = 10,
				CategoryId = 2,
				BrandId = 3,
				Category = new ProductCategory { Id = 2, Name = "Cat" },
				Brand = new Brand { Id = 3, Name = "Brand" },
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
				}
			};
			var products = new List<Product> { product }.BuildMock();
			_mockProductRepo.Setup(r => r.GetAllAttached()).Returns(products);

			var result = await _service.GetEditableProductByIdAsync(1);
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Id, Is.EqualTo(1));
			Assert.That(result.Name, Is.EqualTo("Test"));
			Assert.That(result.Material, Is.EqualTo("Leather"));
			Assert.That(result.Fit, Is.EqualTo("Regular"));
		}

		[Test]
		public async Task EditProductAsync_ShouldReturnFalse_WhenModelIsNull()
		{
			var result = await _service.EditProductAsync(null);
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task EditProductAsync_ShouldReturnFalse_WhenProductNotFound()
		{
			var products = new List<Product>().BuildMock();
			_mockProductRepo.Setup(r => r.GetAllAttached()).Returns(products);
			var model = new EditProductInputModel { Id = 1, CategoryId = 2, BrandId = 3, AverageRating = "4.5" };

			_mockCategoryRepo.Setup(r => r.SingleOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ProductCategory, bool>>>()))
				.ReturnsAsync(new ProductCategory { Id = 2, Name = "Cat" });
			_mockBrandRepo.Setup(r => r.SingleOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Brand, bool>>>()))
				.ReturnsAsync(new Brand { Id = 3, Name = "Brand" });

			var result = await _service.EditProductAsync(model);
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task EditProductAsync_ShouldReturnFalse_WhenCategoryNotFound()
		{
			var product = new Product { Id = 1, ProductDetails = new ProductDetails(), Category = new ProductCategory(), Brand = new Brand() };
			var products = new List<Product> { product }.BuildMock();
			_mockProductRepo.Setup(r => r.GetAllAttached()).Returns(products);
			var model = new EditProductInputModel { Id = 1, CategoryId = 2, BrandId = 3, AverageRating = "4.5" };

			_mockCategoryRepo.Setup(r => r.SingleOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ProductCategory, bool>>>()))
				.ReturnsAsync((ProductCategory)null);

			var result = await _service.EditProductAsync(model);
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task EditProductAsync_ShouldReturnFalse_WhenAverageRatingIsInvalid()
		{
			var product = new Product { Id = 1, ProductDetails = new ProductDetails(), Category = new ProductCategory(), Brand = new Brand() };
			var products = new List<Product> { product }.BuildMock();
			_mockProductRepo.Setup(r => r.GetAllAttached()).Returns(products);
			var model = new EditProductInputModel { Id = 1, CategoryId = 2, BrandId = 3, AverageRating = "invalid" };

			_mockCategoryRepo.Setup(r => r.SingleOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ProductCategory, bool>>>()))
				.ReturnsAsync(new ProductCategory { Id = 2, Name = "Cat" });

			var result = await _service.EditProductAsync(model);
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task EditProductAsync_ShouldReturnTrue_WhenEditSucceeds()
		{
			var product = new Product
			{
				Id = 1,
				ProductDetails = new ProductDetails(),
				Category = new ProductCategory { Products = new List<Product>() },
				Brand = new Brand { Products = new List<Product>() }
			};
			product.Category.Products.Add(product);
			product.Brand.Products.Add(product);

			var products = new List<Product> { product }.BuildMock();
			_mockProductRepo.Setup(r => r.GetAllAttached()).Returns(products);

			var model = new EditProductInputModel
			{
				Id = 1,
				Name = "Test",
				Description = "Desc",
				ImageUrl = "img",
				Price = 10.5m,
				DiscountPrice = 5.0m,
				IsActive = true,
				StockQuantity = 100,
				AverageRating = "4.5",
				TotalRatings = 10,
				CategoryId = 2,
				BrandId = 3,
				Material = "Leather",
				Color = "Black",
				Gender = "Unisex",
				SizeGuideUrl = "url",
				CountryOfOrigin = "USA",
				CareInstructions = "Care",
				Weight = 1.2m,
				Fit = "Regular",
				Style = "Sport"
			};

			_mockCategoryRepo.Setup(r => r.SingleOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ProductCategory, bool>>>()))
				.ReturnsAsync(new ProductCategory { Id = 2, Name = "Cat", Products = new List<Product>() });
			_mockBrandRepo.Setup(r => r.SingleOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Brand, bool>>>()))
				.ReturnsAsync(new Brand { Id = 3, Name = "Brand", Products = new List<Product>() });

			_mockProductRepo.Setup(r => r.UpdateAsync(product)).ReturnsAsync(true);

			var result = await _service.EditProductAsync(model);
			Assert.That(result, Is.True);
		}

		[Test]
		public async Task GetProductDetailsByIdAsync_ShouldReturnNull_WhenIdIsNull()
		{
			var result = await _service.GetProductDetailsByIdAsync(null);
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetProductDetailsByIdAsync_ShouldReturnNull_WhenProductNotFound()
		{
			var products = new List<Product>().BuildMock();
			_mockProductRepo.Setup(r => r.GetAllAttached()).Returns(products);

			var result = await _service.GetProductDetailsByIdAsync(1);
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetProductDetailsByIdAsync_ShouldReturnViewModel_WhenProductFound()
		{
			var product = new Product
			{
				Id = 1,
				Name = "Test",
				Description = "Desc",
				ImageUrl = "img",
				Price = 10.5m,
				DiscountPrice = 5.0m,
				IsActive = true,
				StockQuantity = 100,
				AverageRating = 4.5,
				TotalRatings = 10,
				Category = new ProductCategory { Name = "Cat" },
				Brand = new Brand { Name = "Brand" },
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
				}
			};
			var products = new List<Product> { product }.BuildMock();
			_mockProductRepo.Setup(r => r.GetAllAttached()).Returns(products);

			var result = await _service.GetProductDetailsByIdAsync(1);
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Id, Is.EqualTo(1));
			Assert.That(result.Name, Is.EqualTo("Test"));
			Assert.That(result.Category, Is.EqualTo("Cat"));
			Assert.That(result.Brand, Is.EqualTo("Brand"));
			Assert.That(result.Material, Is.EqualTo("Leather"));
			Assert.That(result.Fit, Is.EqualTo("Regular"));
		}

		[Test]
		public async Task GetProductsIdsAndNamesAsync_ReturnsMappedPromotionProductViewModels()
		{
			var products = new List<Product>
			{
				new Product { Id = 1, Name = "Product1" },
				new Product { Id = 2, Name = "Product2" }
			}.BuildMock();

			_mockProductRepo.Setup(r => r.GetAllAttached()).Returns(products);

			var result = await _service.GetProductsIdsAndNamesAsync();

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count(), Is.EqualTo(2));
			Assert.That(result.First().Id, Is.EqualTo(1));
			Assert.That(result.First().Name, Is.EqualTo("Product1"));
			Assert.That(result.Last().Id, Is.EqualTo(2));
			Assert.That(result.Last().Name, Is.EqualTo("Product2"));
		}

		[Test]
		public async Task GetProductsIdsAndNamesAsync_ReturnsEmpty_WhenNoProducts()
		{
			var products = new List<Product>().BuildMock();
			_mockProductRepo.Setup(r => r.GetAllAttached()).Returns(products);

			var result = await _service.GetProductsIdsAndNamesAsync();

			Assert.That(result, Is.Empty);
		}

		[Test]
		public void GetGendersForProductDetails_ReturnsSelectListItems_ForAllowedGenders()
		{
			var result = _service.GetGendersForProductDetails().ToList();

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count, Is.EqualTo(3));
			Assert.That(result[0].Value, Is.EqualTo("Male"));
			Assert.That(result[1].Value, Is.EqualTo("Female"));
			Assert.That(result[2].Value, Is.EqualTo("Unisex"));
			Assert.That(result.All(x => x.Value == x.Text), Is.True);
		}

	}
}
using MockQueryable;
using Moq;

using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Services.Core;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.Home.Partial;

namespace OnlineStore.Services.Tests
{

	[TestFixture]
	public class ProductPromotionServiceTests
	{
		private Mock<IProductPromotionRepository> _promotionRepositoryMock;
		private IProductPromotionService _productPromotionService;

		[SetUp]
		public void Setup()
		{
			this._promotionRepositoryMock = new Mock<IProductPromotionRepository>(MockBehavior.Strict);
			this._productPromotionService = new ProductPromotionService(this._promotionRepositoryMock.Object);
		}

		[Test]
		public void IsSetupWorking()
		{
			Assert.Pass();
		}

		[Test]
		public async Task GetAllProductPromotionsViewModelShouldReturnEmptyCollection()
		{
			List<ProductPromotion> emptyPromotionList = new();
			IQueryable<ProductPromotion> emptyPromotionQueryable =
						emptyPromotionList.BuildMock();

			this._promotionRepositoryMock
							.Setup(pr => pr.GetAllAttached())
							.Returns(emptyPromotionQueryable);

			IEnumerable<ProductPromotionViewModel> promotionVm = await this._productPromotionService
								.GetProductsPromotionsAsync();

			Assert.That(promotionVm, Is.Not.Null);
			Assert.That(emptyPromotionList.Count(), Is.EqualTo(promotionVm.Count()));
		}

		[Test]
		public async Task GetAllProductsPromotionsViewModelShouldReturnSameCollecionDataCountWhenNoEmpty()
		{
			List<ProductPromotion> promotionList = new()
			{
				new ProductPromotion()
				{
					Id = 1,
					ProductId = 2,
					Product = new Product
					{
						Id = 2,
						ImageUrl = "nike.jpg",
						Name = "Nike Air Max",
						Price = 120m,
						DiscountPrice = 60m
					},
					PromotionPrice = 50.00m,
					Label = "Summar Promotion",
					StartDate = DateTime.Now,
					ExpDate = DateTime.Now.AddDays(30),
					IsDeleted = false
				},
				new ProductPromotion()
				{
					Id = 2,
					ProductId = 3,
					Product = new Product
					{
						Id = 3,
						ImageUrl = "addidas.jpg",
						Name = "Addidas tee",
						Price = 150m,
						DiscountPrice = 80m
					},
					PromotionPrice = 60.00m,
					Label = "Winter Promotion",
					StartDate = DateTime.Now,
					ExpDate = DateTime.Now.AddDays(20),
					IsDeleted = false
				}
			};

			IQueryable<ProductPromotion> promotionQueryable =
						promotionList.BuildMock();

			this._promotionRepositoryMock
							.Setup(pr => pr.GetAllAttached())
							.Returns(promotionQueryable);

			IEnumerable<ProductPromotionViewModel> promotionVm = await this._productPromotionService
								.GetProductsPromotionsAsync();

			Assert.That(promotionVm, Is.Not.Null);
			Assert.That(promotionList.Count, Is.EqualTo(promotionVm.Count()), 
						"The promotionVm should have the same count as the promotinList!");
		}

		[Test]
		public async Task GetAllProductsPromotionsViewModelShouldReturnSameDataInViewModels()
		{
			List<ProductPromotion> promotionList = new()
			{
				new ProductPromotion()
				{
					Id = 1,
					ProductId = 2,
					Product = new Product
					{
						Id = 2,
						ImageUrl = "nike.jpg",
						Name = "Nike Air Max",
						Price = 120m,
						DiscountPrice = 60m
					},
					PromotionPrice = 50.00m,
					Label = "Summar Promotion",
					StartDate = DateTime.Now,
					ExpDate = DateTime.Now.AddDays(30),
					IsDeleted = false
				},
				new ProductPromotion()
				{
					Id = 2,
					ProductId = 3,
					Product = new Product
					{
						Id = 3,
						ImageUrl = "addidas.jpg",
						Name = "Addidas tee",
						Price = 150m,
						DiscountPrice = 80m
					},
					PromotionPrice = 60.00m,
					Label = "Winter Promotion",
					StartDate = DateTime.Now,
					ExpDate = DateTime.Now.AddDays(20),
					IsDeleted = false
				}
			};

			IQueryable<ProductPromotion> promotionQueryable =
						promotionList.BuildMock();

			this._promotionRepositoryMock
							.Setup(pr => pr.GetAllAttached())
							.Returns(promotionQueryable);

			IEnumerable<ProductPromotionViewModel> promotionsViewModels = await this._productPromotionService
								.GetProductsPromotionsAsync();

			Assert.That(promotionsViewModels, Is.Not.Null);
			Assert.That(promotionList.Count, Is.EqualTo(promotionsViewModels.Count()),
						"The promotionVm should have the same count as the promotinList!");

			foreach (var promotion in promotionList)
			{
				ProductPromotionViewModel? promotionVm = promotionsViewModels
										.FirstOrDefault(p => p.ProductId == promotion.ProductId);

				string expectedPercent = Math
											.Round((((promotion.Product.Price - promotion.Product.DiscountPrice) / promotion.Product.Price) * 100)!.Value)
												.ToString()
													.TrimEnd('0')
														.TrimEnd('.');

				Assert.That(promotionVm, Is.Not.Null);
				Assert.That(promotion.Product.Name, Is.EqualTo(promotionVm.ProductName));
				Assert.That(promotion.Label, Is.EqualTo(promotionVm.Label));
				Assert.That(promotion.Product.ImageUrl, Is.EqualTo(promotionVm.ImageUrl));
				Assert.That(promotion.ExpDate.ToString("yyyy-MM-dd"), Is.EqualTo(promotionVm.ExpDate));
				Assert.That(expectedPercent, Is.EqualTo(promotionVm.Percent));
			}
		}

		[Test]
		public async Task GetAllProductsPromotionsViewModelShouldReturnEmptyCollectionWhenInvalidDataIsPassed()
		{
			List<ProductPromotion> promotionList = new()
			{
				new ProductPromotion()
				{
					Id = 1,
					ProductId = 2,
					Product = new Product
					{
						Id = 2,
						ImageUrl = "nike.jpg",
						Name = "Nike Air Max",
						Price = 120m,
						DiscountPrice = 60m
					},
					PromotionPrice = 50.00m,
					Label = "Summar Promotion",
					StartDate = DateTime.Now,
					ExpDate = DateTime.Now.AddDays(30),
					IsDeleted = true
				},
				new ProductPromotion()
				{
					Id = 2,
					ProductId = 3,
					Product = new Product
					{
						Id = 3,
						ImageUrl = "addidas.jpg",
						Name = "Addidas tee",
						Price = 150m,
						DiscountPrice = 80m
					},
					PromotionPrice = 60.00m,
					Label = "Winter Promotion",
					StartDate = DateTime.Now,
					ExpDate = DateTime.Now,
					IsDeleted = false
				}
			};

			IQueryable<ProductPromotion> promotionQueryable =
						promotionList.BuildMock();

			this._promotionRepositoryMock
							.Setup(pr => pr.GetAllAttached())
							.Returns(promotionQueryable);

			IEnumerable<ProductPromotionViewModel> promotionsViewModels = await this._productPromotionService
								.GetProductsPromotionsAsync();

			Assert.That(promotionsViewModels, Is.Not.Null);
			Assert.That(promotionList.Count, Is.Not.EqualTo(promotionsViewModels.Count()),
						"The promotionVm should not have the same count as the promotinList!");
			Assert.That(promotionsViewModels.Count(), Is.EqualTo(0));
		}
	}
}
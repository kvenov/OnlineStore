using Microsoft.AspNetCore.Mvc.Rendering;
using MockQueryable;
using Moq;
using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Services.Core;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.Layout;

using static OnlineStore.Data.Common.Constants.EntityConstants.ProductDetails;

namespace OnlineStore.Services.Tests
{

	[TestFixture]
	public class ProductCategoryServiceTests
	{
		private Mock<IRepository<ProductCategory, int>> _productCategoryRepositoryMock;
		private IProductCategoryService _productCategoryService;

		[SetUp]
		public void Setup()
		{
			this._productCategoryRepositoryMock = new Mock<IRepository<ProductCategory, int>>();
			this._productCategoryService = new ProductCategoryService(this._productCategoryRepositoryMock.Object);
		}

		[Test]
		public void IsSetupWorking()
		{
			Assert.Pass();
		}

		[Test]
		public async Task GetAllProductCategoriesViewModelShouldReturnEmptyCollectionWhenNoCategoriesPassed()
		{
			List<ProductCategory> emptyCategoryList = new List<ProductCategory>();
			IQueryable<ProductCategory> emptyCategoryQueryable =
								emptyCategoryList.BuildMock();

			this._productCategoryRepositoryMock
							.Setup(pc => pc.GetAllAttached())
							.Returns(emptyCategoryQueryable);

			IEnumerable<SelectListItem> productCategoriesVm = await this._productCategoryService
									.GetAllProductCategoriesIdsAndNamesAsync();

			Assert.That(productCategoriesVm, Is.Not.Null);
			Assert.That(emptyCategoryList.Count, Is.EqualTo(productCategoriesVm.Count()));
		}

		[Test]
		public async Task GetAllProductCategoriesViewModelShouldReturnSameCollectionCountWhenProductCategoriesArePassed()
		{
			List<ProductCategory> categoryList = new()
			{
				new ProductCategory()
				{
					Id = 1,
					Name = "Shoes"
				},
				new ProductCategory()
				{
					Id = 2,
					Name = "Cloths"
				}
			};
			IQueryable<ProductCategory> categoryQueryable =
								categoryList.BuildMock();

			this._productCategoryRepositoryMock
							.Setup(pc => pc.GetAllAttached())
							.Returns(categoryQueryable);

			IEnumerable<SelectListItem> productCategoriesVm = await this._productCategoryService
									.GetAllProductCategoriesIdsAndNamesAsync();

			Assert.That(productCategoriesVm, Is.Not.Null);
			Assert.That(categoryList.Count, Is.EqualTo(productCategoriesVm.Count()));
		}

		[Test]
		public async Task GetAllProductCategoriesViewModelShouldReturnSameCollectionDataWhenValidProductCategoriesArePassed()
		{
			List<ProductCategory> categoryList = new()
			{
				new ProductCategory()
				{
					Id = 1,
					Name = "Shoes"
				},
				new ProductCategory()
				{
					Id = 2,
					Name = "Cloths"
				}
			};
			IQueryable<ProductCategory> categoryQueryable =
								categoryList.BuildMock();

			this._productCategoryRepositoryMock
							.Setup(pc => pc.GetAllAttached())
							.Returns(categoryQueryable);

			IEnumerable<SelectListItem> productCategoriesVm = await this._productCategoryService
									.GetAllProductCategoriesIdsAndNamesAsync();

			Assert.That(productCategoriesVm, Is.Not.Null);
			Assert.That(categoryList.Count, Is.EqualTo(productCategoriesVm.Count()));

			foreach (var category in categoryList)
			{
				SelectListItem? categotyVm = productCategoriesVm
								.FirstOrDefault(pc => pc.Value == category.Id.ToString());

				Assert.That(categotyVm, Is.Not.Null);
				Assert.That(categotyVm.Value, Is.EqualTo(category.Id.ToString()));
				Assert.That(categotyVm.Text, Is.EqualTo(category.Name));
			}
		}

		[Test]
		public async Task GetLayoutCategoryMenuViewModelShouldReturnEmptyCollectionWhenNoCategoriesPassed()
		{
			List<ProductCategory> emptyCategoryList = new List<ProductCategory>();
			IQueryable<ProductCategory> emptyCategoryQueryable =
								emptyCategoryList.BuildMock();

			this._productCategoryRepositoryMock
							.Setup(pc => pc.GetAllAttached())
							.Returns(emptyCategoryQueryable);

			IEnumerable<MenuViewModel> menuVm = await this._productCategoryService
									.GetLayoutCategoryMenuAsync();
			int allowedGendersCount = AllowedGenders.Length;

			Assert.That(menuVm, Is.Not.Null);
			Assert.That(menuVm.Count(), Is.EqualTo(allowedGendersCount));

			foreach (var menu in menuVm)
			{
				int expectedCategoryGroupsCount = 0;

				Assert.That(menu.CategoryGroups, Is.Not.Null);
				Assert.That(menu.CategoryGroups, Is.Empty);
				Assert.That(expectedCategoryGroupsCount, Is.EqualTo(menu.CategoryGroups.Count));
			}
		}

		[Test]
		public async Task GetLayoutCategoryMenuViewModelShouldReturnCollectionWithNoSubCategoriesInCategoryGroupsWhenNoSubcategoriesArePassedInTheCategoriesCollection()
		{
			List<ProductCategory> categoryList = new()
			{
				new ProductCategory()
				{
					Id = 1,
					Name = "Shoes",
					ParentCategory = null,
					Subcategories = new HashSet<ProductCategory>()
				},
				new ProductCategory()
				{
					Id = 2,
					Name = "Cloths",
					ParentCategory = null,
					Subcategories = new HashSet<ProductCategory>()
				}
			};
			IQueryable<ProductCategory> categoryQueryable =
								categoryList.BuildMock();

			this._productCategoryRepositoryMock
							.Setup(pc => pc.GetAllAttached())
							.Returns(categoryQueryable);

			IEnumerable<MenuViewModel> menuVm = await this._productCategoryService
									.GetLayoutCategoryMenuAsync();

			int allowedGendersCount = AllowedGenders.Length;

			Assert.That(menuVm, Is.Not.Null);
			Assert.That(menuVm.Count(), Is.EqualTo(allowedGendersCount));

			foreach (var menu in menuVm)
			{
				int expectedCategoryGroupsCount = categoryList.Count;

				Assert.That(menu.CategoryGroups, Is.Not.Null);
				Assert.That(expectedCategoryGroupsCount, Is.EqualTo(menu.CategoryGroups.Count));

				foreach (var categoryGroup in menu.CategoryGroups)
				{
					Assert.That(categoryGroup.ParentCategory, Is.Not.Null);

					ProductCategory? category = categoryList
											.FirstOrDefault(c => c.Name.ToLower() == categoryGroup.ParentCategory.ToLower());

					Assert.That(category, Is.Not.Null);

					int expectedSubcategoriesCount = category.Subcategories.Count;
					Assert.That(categoryGroup.Subcategories, Is.Not.Null);
					Assert.That(expectedSubcategoriesCount, Is.EqualTo(categoryGroup.Subcategories.Count));

					Assert.That(category.Name, Is.EqualTo(categoryGroup.ParentCategory));
				}
			}
		}

		[Test]
		public async Task GetLayoutCategoryMenuViewModelShouldReturnSameCollectionCountWhenCategoriesArePassed()
		{
			List<ProductCategory> firstSubCategories = new()
			{
				new ProductCategory()
				{
					Id = 5,
					Name = "Sporty Shoes",
				},
				new ProductCategory()
				{
					Id = 6,
					Name = "LifeStyle Shoes",
				}
			};

			List<ProductCategory> secondSubCategories = new()
			{
				new ProductCategory()
				{
					Id = 11,
					Name = "Jeans",
				},
				new ProductCategory()
				{
					Id = 22,
					Name = "T-Shirts",
				}
			};

			List<ProductCategory> categoryList = new()
			{
				new ProductCategory()
				{
					Id = 1,
					Name = "Shoes",
					ParentCategory = null,
					Subcategories = firstSubCategories
				},
				new ProductCategory()
				{
					Id = 2,
					Name = "Cloths",
					ParentCategory = null,
					Subcategories = secondSubCategories
				}
			};

			IQueryable<ProductCategory> categoryQueryable =
								categoryList.BuildMock();

			this._productCategoryRepositoryMock
							.Setup(pc => pc.GetAllAttached())
							.Returns(categoryQueryable);

			IEnumerable<MenuViewModel> menuVm = await this._productCategoryService
									.GetLayoutCategoryMenuAsync();

			int allowedGendersCount = AllowedGenders.Length;
			Assert.That(menuVm, Is.Not.Null);
			Assert.That(menuVm.Count(), Is.EqualTo(allowedGendersCount));

			foreach (var menu in menuVm)
			{
				int expectedCategoryGroupsCount = categoryList.Count;

				Assert.That(menu.CategoryGroups, Is.Not.Null);
				Assert.That(menu.CategoryGroups, Is.Not.Empty);
				Assert.That(expectedCategoryGroupsCount, Is.EqualTo(menu.CategoryGroups.Count));

				foreach (var categoryGroup in menu.CategoryGroups)
				{
					Assert.That(categoryGroup, Is.Not.Null);

					Assert.That(categoryGroup.ParentCategory, Is.Not.Null);
					Assert.That(categoryGroup.Subcategories, Is.Not.Null);
					Assert.That(categoryGroup.Subcategories, Is.Not.Empty);

					ProductCategory? category = categoryList
										.FirstOrDefault(c => c.Name.ToLower() == categoryGroup.ParentCategory.ToLower());

					Assert.That(category, Is.Not.Null);
					Assert.That(category.Name.ToLower(), Is.EqualTo(categoryGroup.ParentCategory.ToLower()));

					var expectedSubcategoryNames = category.Subcategories.Select(sc => sc.Name).ToList();

					Assert.That(categoryGroup.Subcategories.Count, Is.EqualTo(expectedSubcategoryNames.Count));
					Assert.That(categoryGroup.Subcategories, Is.EquivalentTo(expectedSubcategoryNames));

				}
			}
		}
	}
}

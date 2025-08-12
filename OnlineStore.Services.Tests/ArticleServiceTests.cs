using MockQueryable;
using Moq;
using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Services.Core;

namespace OnlineStore.Services.Tests
{
	[TestFixture]
	public class ArticleServiceTests
	{
		private Mock<IRepository<Article, int>> _repositoryMock;
		private ArticleService _service;

		[SetUp]
		public void SetUp()
		{
			_repositoryMock = new Mock<IRepository<Article, int>>();
			_service = new ArticleService(_repositoryMock.Object);
		}

		[Test]
		public void IsSetupWorking()
		{
			Assert.Pass();
		}

		[Test]
		public async Task GetUserReviewsAsync_ShouldReturnMappedReviews_WhenArticlesHaveAuthors()
		{
			// Arrange
			var articles = new List<Article>
			{
				new Article
				{
					Id = 1,
					Content = "Great product!",
					AuthorId = "user-1",
					Author = new ApplicationUser
					{
						Id = "user-1",
						UserName = "john_doe"
					}
				},
				new Article
				{
					Id = 2,
					Content = "Love this item",
					AuthorId = "user-2",
					Author = new ApplicationUser
					{
						Id = "user-2",
						UserName = "jane_doe"
					}
				}
			};

			IQueryable<Article> mockQueryable = articles.BuildMock();
			_repositoryMock
				.Setup(r => r.GetAllAttached())
				.Returns(mockQueryable);

			// Act
			var result = await _service.GetUserReviewsAsync();

			// Assert
			Assert.That(result, Is.Not.Null);
			var reviewList = result.ToList();
			Assert.That(reviewList.Count, Is.EqualTo(2));

			Assert.Multiple(() =>
			{
				Assert.That(reviewList[0].UserId, Is.EqualTo("user-1"));
				Assert.That(reviewList[0].Username, Is.EqualTo("john_doe"));
				Assert.That(reviewList[0].Content, Is.EqualTo("Great product!"));

				Assert.That(reviewList[1].UserId, Is.EqualTo("user-2"));
				Assert.That(reviewList[1].Username, Is.EqualTo("jane_doe"));
				Assert.That(reviewList[1].Content, Is.EqualTo("Love this item"));
			});

			_repositoryMock.Verify(r => r.GetAllAttached(), Times.Once);
		}

		[Test]
		public async Task GetUserReviewsAsync_ShouldSetUsernameNull_WhenAuthorIsNull()
		{
			// Arrange
			var articles = new List<Article>
			{
				new Article
				{
					Id = 3,
					Content = "Anonymous review",
					AuthorId = null,
					Author = null
				}
			};

			IQueryable<Article> mockQueryable = articles.BuildMock();
			_repositoryMock
				.Setup(r => r.GetAllAttached())
				.Returns(mockQueryable);

			// Act
			var result = await _service.GetUserReviewsAsync();

			// Assert
			Assert.That(result, Is.Not.Null);
			var reviewList = result.ToList();
			Assert.That(reviewList.Count, Is.EqualTo(1));
			Assert.That(reviewList[0].UserId, Is.Null);
			Assert.That(reviewList[0].Username, Is.Null);
			Assert.That(reviewList[0].Content, Is.EqualTo("Anonymous review"));

			_repositoryMock.Verify(r => r.GetAllAttached(), Times.Once);
		}

		[Test]
		public async Task GetUserReviewsAsync_ShouldReturnEmptyList_WhenNoArticles()
		{
			var articles = new List<Article>(); // empty
			IQueryable<Article> mockQueryable = articles.BuildMock();
			_repositoryMock
				.Setup(r => r.GetAllAttached())
				.Returns(mockQueryable);

			// Act
			var result = await _service.GetUserReviewsAsync();

			// Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.Empty);

			_repositoryMock.Verify(r => r.GetAllAttached(), Times.Once);
		}
	}
}
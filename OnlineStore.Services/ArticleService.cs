using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.Home.Partial;

namespace OnlineStore.Services.Core
{
	public class ArticleService : IArticleService
	{
		private readonly IRepository<Article, int> _repository;

		public ArticleService(IRepository<Article, int> repository)
		{
			this._repository = repository;
		}

		public async Task<IEnumerable<UserReviewViewModel>> GetUserReviewsAsync()
		{
			IEnumerable<UserReviewViewModel> articles = await this._repository
							.GetAllAttached()
							.AsNoTracking()
							.Include(a => a.Author)
							.Select(a => new UserReviewViewModel()
							{
								UserId = a.AuthorId,
								Content = a.Content,
								Username = a.Author != null ? a.Author.UserName : null,
							})
							.ToListAsync();

			return articles;
		}
	}
}

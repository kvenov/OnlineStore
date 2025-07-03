using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.Home.Partial;

namespace OnlineStore.Services.Core
{
	public class ArticleService : IArticleService
	{
		private readonly ApplicationDbContext _context;

		public ArticleService(ApplicationDbContext context)
		{
			this._context = context;
		}

		public async Task<IEnumerable<UserReviewViewModel>> GetUserReviewsAsync()
		{
			IEnumerable<UserReviewViewModel> articles = await this._context
							.Articles
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

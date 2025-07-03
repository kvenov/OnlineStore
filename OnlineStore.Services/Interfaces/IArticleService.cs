using OnlineStore.Web.ViewModels.Home.Partial;

namespace OnlineStore.Services.Core.Interfaces
{
	public interface IArticleService
	{

		Task<IEnumerable<UserReviewViewModel>> GetUserReviewsAsync();
	}
}

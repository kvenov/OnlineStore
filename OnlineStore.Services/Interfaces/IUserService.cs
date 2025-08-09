using OnlineStore.Web.ViewModels.User;

namespace OnlineStore.Services.Core.Interfaces
{
	public interface IUserService
	{

		Task<AccountSettingsViewModel?> GetUserSettingsAsync(string? userId);
	}
}

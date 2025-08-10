using OnlineStore.Web.ViewModels.User;

namespace OnlineStore.Services.Core.Interfaces
{
	public interface IUserService
	{

		Task<AccountSettingsViewModel?> GetUserSettingsAsync(string? userId);

		Task<(bool result, string message)> ChangeUserProfileAsync(UpdateProfileInputModel? model, string? userId);

		Task<(bool result, string message)> ChangeUserPasswordAsync(ChangePasswordInputModel? model, string? userId);

		Task<(bool result, string message)> ChangeUserAddressesAsync(UpdateAddressesInputModel? model, string? userId);

		Task<(bool result, string message)> ChangeUserPaymentDataAsync(UpdatePaymentInputModel? model, string? userId);

	}
}

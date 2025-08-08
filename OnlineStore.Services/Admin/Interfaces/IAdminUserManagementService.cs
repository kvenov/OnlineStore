using OnlineStore.Web.ViewModels.Admin.UserManagement;

namespace OnlineStore.Services.Core.Admin.Interfaces
{
	public interface IAdminUserManagementService
	{

		Task<IEnumerable<UserManagementViewModel>> GetAllUsersAsync(string userId);
	}
}

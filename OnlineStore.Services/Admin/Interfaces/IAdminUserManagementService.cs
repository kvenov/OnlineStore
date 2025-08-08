using OnlineStore.Web.ViewModels.Admin.UserManagement;

namespace OnlineStore.Services.Core.Admin.Interfaces
{
	public interface IAdminUserManagementService
	{

		Task<IEnumerable<UserManagementViewModel>> GetAllUsersAsync(string userId);

		Task<bool> AssignUserToRoleAsync(string? userId, string? role);

		Task<bool> RemoveRoleFromUserAsync(string? userId, string? role);

		Task<bool> SoftDeleteUserAsync(string? userId);

		Task<bool> RenewUserAsync(string? userId);
	}
}

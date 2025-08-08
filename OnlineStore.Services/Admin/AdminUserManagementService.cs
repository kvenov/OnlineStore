using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models;
using OnlineStore.Services.Core.Admin.Interfaces;
using OnlineStore.Web.ViewModels.Admin.UserManagement;

namespace OnlineStore.Services.Core.Admin
{
	public class AdminUserManagementService : IAdminUserManagementService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public AdminUserManagementService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			this._userManager = userManager;
			this._roleManager = roleManager;
		}

		public async Task<IEnumerable<UserManagementViewModel>> GetAllUsersAsync(string userId)
		{
			var users = await this._userManager
					.Users
					.Where(u => u.Id.ToLower() != userId.ToLower())
					.ToListAsync();

			var userViewModels = new List<UserManagementViewModel>();

			foreach (var user in users)
			{
				var roles = await this._userManager.GetRolesAsync(user);

				userViewModels.Add(new UserManagementViewModel
				{
					Id = user.Id,
					Username = user.UserName!,
					FullName = user.FullName,
					Email = user.Email!,
					Roles = roles
				});
			}

			return userViewModels;
		}
	}
}

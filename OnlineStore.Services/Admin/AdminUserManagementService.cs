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

		public async Task<bool> AssignUserToRoleAsync(string? userId, string? role)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(userId);
			ArgumentException.ThrowIfNullOrWhiteSpace(role);

			ApplicationUser? user = await this._userManager
							.FindByIdAsync(userId);

			if (user == null)
				throw new 
					InvalidOperationException("The User cannot be found with the provided credentials!");

			bool roleExist = await this._roleManager.RoleExistsAsync(role);

			if (!roleExist)
				throw new ArgumentException("The provided role is not valid app role!");

			try
			{
				await this._userManager.AddToRoleAsync(user, role);

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				throw new InvalidOperationException("Something went wrong while assigning role to User!");
			}
		}

		public async Task<bool> RemoveRoleFromUserAsync(string? userId, string? role)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(userId);
			ArgumentException.ThrowIfNullOrWhiteSpace(role);

			ApplicationUser? user = await this._userManager
							.FindByIdAsync(userId);

			if (user == null)
				throw new
					InvalidOperationException("The User cannot be found with the provided credentials!");

			bool roleExist = await this._roleManager.RoleExistsAsync(role);

			if (!roleExist)
				throw new ArgumentException("The provided role is not valid app role!");

			try
			{
				await this._userManager.RemoveFromRoleAsync(user, role);

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				throw new InvalidOperationException("Something went wrong while removing role from User!");
			}
		}

		public async Task<bool> SoftDeleteUserAsync(string? userId)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(userId);

			ApplicationUser? user = await this._userManager
								.FindByIdAsync(userId);

			if (user == null)
				throw new InvalidOperationException("User with the provided credentials cannot be found!");

			try
			{
				//This is expected to set the Application User IsDeleted prop to true.
				await this._userManager.DeleteAsync(user);

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				throw new InvalidOperationException("An Error occured while deleting the User!");
			}
		}

		public async Task<bool> RenewUserAsync(string? userId)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(userId);

			ApplicationUser? user = await this._userManager
								.Users
								.IgnoreQueryFilters()
								.SingleOrDefaultAsync(u => u.Id.ToLower() == userId.ToLower());

			if (user == null)
				throw new InvalidOperationException("User with the provided credentials cannot be found!");

			try
			{
				user.IsDeleted = false;
				await this._userManager.UpdateAsync(user);

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				throw new InvalidOperationException("An Error occured while renewed the User!");
			}
		}

		public async Task<IEnumerable<UserManagementViewModel>> GetAllUsersAsync(string userId)
		{
			var users = await this._userManager
					.Users
					.IgnoreQueryFilters()
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
					Roles = roles,
					IsDeleted = user.IsDeleted,
				});
			}

			return userViewModels;
		}

	}
}
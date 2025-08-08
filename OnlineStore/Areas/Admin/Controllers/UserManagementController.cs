using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Core.Admin.Interfaces;
using OnlineStore.Web.ViewModels.Admin.UserManagement;

namespace OnlineStore.Web.Areas.Admin.Controllers
{
	public class UserManagementController : BaseAdminController
	{
		private readonly IAdminUserManagementService _adminUserManagementService;

		private readonly ILogger<UserManagementController> _logger;

		public UserManagementController(ILogger<UserManagementController> logger, IAdminUserManagementService adminUserManagementService)
		{
			this._adminUserManagementService = adminUserManagementService;
			this._logger = logger;
		}

		public async Task<IActionResult> Index()
		{
			try
			{
				string adminId = this.GetUserId()!;

				IEnumerable<UserManagementViewModel> users = await this._adminUserManagementService
										.GetAllUsersAsync(adminId);

				return View(users);

			}
			catch (Exception ex)
			{
				this._logger.LogError(ex, "An Error occured while getting the Application Users data!");
				
				return this.RedirectToAction(nameof(Index), "Home");
			}
		}
	}
}

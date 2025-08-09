using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.User;

namespace OnlineStore.Web.Controllers
{
	public class UserController : BaseController
	{
		private readonly ILogger<UserController> _logger;
		private readonly IUserService _userService;

		public UserController(ILogger<UserController> logger, IUserService userService)
		{
			this._logger = logger;
			this._userService = userService;
		}

		public async Task<IActionResult> AccountSettings()
		{
			try
			{
				string userId = this.GetUserId()!;

				if (userId == null) return this.RedirectToAction("Index", "Home");

				AccountSettingsViewModel? model = await this._userService
									.GetUserSettingsAsync(userId);

				if (model == null) return this.RedirectToAction("Index", "Home");

				return View(model);
			}
			catch (Exception ex)
			{
				this._logger.LogError(ex, "Something went wrong while getting the User settings!");

				return this.RedirectToAction("Index", "Home");
			}
		}
	}
}

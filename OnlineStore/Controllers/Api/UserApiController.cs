using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.User;

namespace OnlineStore.Web.Controllers.Api
{

	[AutoValidateAntiforgeryToken]
	public class UserApiController : BaseApiController
	{
		private readonly ILogger<UserApiController> _logger;
		private readonly IUserService _userService;

		public UserApiController(ILogger<UserApiController> logger, IUserService userService)
		{
			this._logger = logger;
			this._userService = userService;
		}

		[HttpPost("change-profile")]
		public async Task<IActionResult> ChangeProfile([FromBody] UpdateProfileInputModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			try
			{
				string? userId = this.GetUserId();

				(bool result, string message) = await this._userService
										.ChangeUserProfileAsync(model, userId);

				return Ok(new
				{
					result,
					message
				});
			}
			catch (Exception ex)
			{
				this._logger.LogError(ex, "Something went wrong while updating the User profile!");

				return BadRequest();
			}
		}

		[HttpPost("change-password")]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordInputModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			try
			{
				string? userId = this.GetUserId();

				(bool result, string message) = await this._userService
										.ChangeUserPasswordAsync(model, userId);

				return Ok(new
				{
					result,
					message
				});
			}
			catch (Exception ex)
			{
				this._logger.LogError(ex, "Something went wrong while updating the User password");

				return BadRequest();
			}
		}

		[HttpPost("change-address")]
		public async Task<IActionResult> ChangeAddress([FromBody] UpdateAddressesInputModel? model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			try
			{
				string? userId = this.GetUserId();

				(bool result, string message) = await this._userService
										.ChangeUserAddressesAsync(model, userId);

				return Ok(new
				{
					result,
					message
				});
			}
			catch (Exception ex)
			{
				this._logger.LogError(ex, "Something went wrong while updating the User address");

				return BadRequest();
			}
		}

		[HttpPost("change-payment-data")]
		public async Task<IActionResult> ChangePaymentData([FromBody] UpdatePaymentInputModel? model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			try
			{
				string? userId = this.GetUserId();

				(bool result, string message) = await this._userService
										.ChangeUserPaymentDataAsync(model, userId);

				return Ok(new
				{
					result,
					message
				});
			}
			catch (Exception ex)
			{
				this._logger.LogError(ex, "Something went wrong while updating the User payment data!");

				return BadRequest();
			}
		}

	}
}

using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Core.Admin.Interfaces;
using OnlineStore.Services.Core.DTO.UserManagement;

namespace OnlineStore.Web.Areas.Admin.Controllers.Api
{

	[AutoValidateAntiforgeryToken]
	public class UserManagementApiController : BaseAdminApiController
	{
		private readonly ILogger<UserManagementApiController> _logger;
		private readonly IAdminUserManagementService _userManagementService;

		public UserManagementApiController(ILogger<UserManagementApiController> logger, IAdminUserManagementService userManagementService)
		{
			this._logger = logger;
			this._userManagementService = userManagementService;
		}

		[HttpPost("assign/{userId}")]
		public async Task<IActionResult> AssignRole(string? userId, [FromBody]AssignRoleDto dto)
		{
			try
			{
				bool isAssigned = await this._userManagementService
									.AssignUserToRoleAsync(userId, dto.Role);

				if (!isAssigned)
				{
					return BadRequest("The role has not been assigned to the User!");
				}

				return Ok(new
				{
					result = isAssigned
				});
			}
			catch (Exception ex)
			{
				this._logger.LogError(ex, "An Error occured while assigning User to role");
				
				return BadRequest();
			}
		}

		[HttpPost("remove/{userId}")]
		public async Task<IActionResult> RemoveRole(string? userId, [FromBody] AssignRoleDto dto)
		{
			try
			{
				bool isRemoved = await this._userManagementService
									.RemoveRoleFromUserAsync(userId, dto.Role);

				if (!isRemoved)
				{
					return BadRequest("The role has not been removed from the User!");
				}

				return Ok(new
				{
					result = isRemoved
				});
			}
			catch (Exception ex)
			{
				this._logger.LogError(ex, "An Error occured while removing role from User!");

				return BadRequest();
			}
		}

		[HttpPost("delete/{userId}")]
		public async Task<IActionResult> DeleteUser(string? userId)
		{
			try
			{
				bool isDeleted = await this._userManagementService
									.SoftDeleteUserAsync(userId);

				if (!isDeleted)
				{
					return BadRequest("The deletion of the User have been unsuccessful!!");
				}

				return Ok(new
				{
					result = isDeleted
				});
			}
			catch (Exception ex)
			{
				this._logger.LogError(ex, "An Error occured while deleting the User!");

				return BadRequest();
			}
		}

		[HttpPost("renew/{userId}")]
		public async Task<IActionResult> RenewUser(string? userId)
		{
			try
			{
				bool isRenewed = await this._userManagementService
									.RenewUserAsync(userId);

				if (!isRenewed)
				{
					return BadRequest("The resumption of the User have been unsuccessful!!");
				}

				return Ok(new
				{
					result = isRenewed
				});
			}
			catch (Exception ex)
			{
				this._logger.LogError(ex, "An Error occured while renewed the User!");

				return BadRequest();
			}
		}
	}
}

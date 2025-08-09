using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OnlineStore.Web.Controllers.Api
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	[AutoValidateAntiforgeryToken]
	public abstract class BaseApiController : ControllerBase
	{

		protected bool IsAuthenticated()
		{
			return User.Identity != null && User.Identity.IsAuthenticated;
		}

		protected string? GetUserId()
		{
			string? userId = null;

			if (IsAuthenticated())
			{
				userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			}

			return userId;
		}

		protected string? GetGuestId()
		{
			string? guestId = null;

			if (!this.IsAuthenticated())
			{
				guestId = this.HttpContext.Items["GuestIdentifier"]?.ToString();
			}

			return guestId;
		}

	}
}

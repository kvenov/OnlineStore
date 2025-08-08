using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static OnlineStore.Common.ApplicationConstants;

namespace OnlineStore.Web.Areas.Admin.Controllers
{

	[Area(AdminRoleName)]
	[Authorize(Roles = AdminRoleName)]
	public class BaseAdminController : Controller
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
	}
}

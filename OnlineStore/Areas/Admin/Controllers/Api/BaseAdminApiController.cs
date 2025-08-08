using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static OnlineStore.Common.ApplicationConstants;

namespace OnlineStore.Web.Areas.Admin.Controllers.Api
{
	[Route("api/[controller]")]
	[ApiController]
	[Area(AdminRoleName)]
	[Authorize(Roles = AdminRoleName)]
	public class BaseAdminApiController : ControllerBase
	{
	}
}

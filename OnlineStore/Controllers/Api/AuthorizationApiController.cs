using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnlineStore.Web.Controllers.Api
{
	public class AuthorizationApiController : BaseApiController
	{

		[HttpGet("auth")]
		[AllowAnonymous]
		public IActionResult IsUserAuth()
		{
			return Ok(new
			{
				isAuthenticated = this.IsAuthenticated()
			});
		}
	}
}

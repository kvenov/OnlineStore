using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using OnlineStore.Data.Models;
using System.Security.Claims;

namespace OnlineStore.Services.Core.Identity
{
	// CustomClaimsPrincipalFactory.cs tells ASP.NET Core to include user roles in the login cookie.
	// With this factory, we ensures that the authenticated cookie contains the user's roles — so they become part of HttpContext.User. 
	public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
	{
		public CustomClaimsPrincipalFactory(
		UserManager<ApplicationUser> userManager,
		RoleManager<IdentityRole> roleManager,
		IOptions<IdentityOptions> optionsAccessor)
		: base(userManager, roleManager, optionsAccessor)
		{ }

		protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
		{
			var identity = await base.GenerateClaimsAsync(user);
			var roles = await UserManager.GetRolesAsync(user);

			foreach (var role in roles)
			{
				identity.AddClaim(new Claim(ClaimTypes.Role, role));
			}

			return identity;
		}

	}
}

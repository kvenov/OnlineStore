using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Data.Models;
using OnlineStore.Services.Core.Interfaces;

namespace OnlineStore.Web.ViewComponents
{
	public class WishlistCountViewComponent : ViewComponent
	{

		private readonly IWishlistService _wishlistService;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public WishlistCountViewComponent(
			IWishlistService wishlistService,
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager)
		{
			this._wishlistService = wishlistService;
			this._userManager = userManager;
			this._signInManager = signInManager;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			if (this._signInManager.IsSignedIn(HttpContext.User) && this.HttpContext.User.IsInRole("User"))
			{
				ApplicationUser? user = await this._userManager.GetUserAsync(HttpContext.User);
				int count = await this._wishlistService.GetWishlistItemsCountAsync(user!.Id);

				return View("_WishlistPartial", count);
			}
			else
			{
				return Content(string.Empty);
			}
		}
	}
}
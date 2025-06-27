using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.Wishlist;

namespace OnlineStore.Web.Controllers
{
	public class WishlistController : BaseController
	{
		private readonly IWishlistService _wishlistService;

		public WishlistController(IWishlistService wishlistService)
		{
			this._wishlistService = wishlistService;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			try
			{
				string userId = this.GetUserId()!;

				WishlistIndexViewModel wishlistModel = await this._wishlistService
						  .GetUserWishlist(userId);

				return View(wishlistModel);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return this.RedirectToAction(nameof(Index), "Home");
			}
		}

		[HttpPost]
		public async Task<IActionResult> EditNote(int? itemId, string? note)
		{
			try
			{
				string userId = GetUserId()!;

				bool isEdited = await this._wishlistService
								.EditNoteAsync(itemId, note, userId);

				if (isEdited)
				{
					return this.RedirectToAction(nameof(Index));
				}
				else
				{
					return this.RedirectToAction(nameof(Index));
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return this.RedirectToAction(nameof(Index), "Home");
			}
		}
	}
}

using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Core.Interfaces;

namespace OnlineStore.Web.Controllers.Api
{
	public class WishlistApiController : BaseApiController
	{
		private readonly IWishlistService _wishlistService;

		public WishlistApiController(IWishlistService wishlistService)
		{
			this._wishlistService = wishlistService;
		}

		[HttpPost("add/{productId}")]
		public async Task<IActionResult> AddToWishlist(int? productId)
		{
			try
			{
				string userId = GetUserId()!;

				if (userId == null)
				{
					return Unauthorized();
				}

				bool isAdded = await this._wishlistService
								.AddProductToWishlist(productId, userId);

				if (isAdded)
				{
					return this.Ok(new { success = true, message = "Product added to wishlist." });
				}
				else
				{
					return this.BadRequest(new { success = false, message = "Product already in wishlist or error occurred." });
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return this.BadRequest();
			}
		}

		[HttpPost("remove/{itemId}")]
		public async Task<IActionResult> Remove(int? itemId)
		{
			try
			{
				string userId = GetUserId()!;

				bool isRemoved = await this._wishlistService
								.RemoveProductFromWishlistAsycn(itemId, userId);

				if (isRemoved)
				{
					return this.Ok(new { success = true, message = "Product successfully removed from wishlist." });
				}
				else
				{
					return this.BadRequest(new { success = false, message = "An error occurred while removing the product from the wishist." });
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return this.BadRequest();
			}
		}

		[HttpGet("count")]
		public async Task<IActionResult> Count()
		{
			try
			{
				string userId = GetUserId()!;

				int count = await this._wishlistService
							.GetWishlistItemsCountAsync(userId);

				return this.Ok(new { data = count , success = true, message = $"{count} products in the wishlist" });
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return BadRequest();
			}
		}
	}
}

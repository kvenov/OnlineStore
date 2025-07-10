using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.DTO.ShoppingCart;
using OnlineStore.Web.ViewModels.ShoppingCart;

namespace OnlineStore.Web.Controllers.Api
{
	[Route("api/[controller]")]
	[ApiController]
	public class ShoppingCartApiController : BaseApiController
	{
		private readonly IShoppingCartService _shoppingCartService;

		public ShoppingCartApiController(IShoppingCartService shoppingCartService)
		{
			this._shoppingCartService = shoppingCartService;
		}

		[AllowAnonymous]
		[HttpPost("add/{productId}")]
		public async Task<IActionResult> AddToCart(int? productId)
		{

			try
			{
				string? userId = this.GetUserId();

				bool isAdded;
				if (userId != null)
				{
					isAdded = await this._shoppingCartService
							.AddToCartForUserAsync(productId, userId);
				}
				else
				{
					isAdded = await this._shoppingCartService
							.AddToCartForGuestAsync(productId, this.HttpContext);
				}

				if (isAdded)
				{
					return Ok(new {result = "The product is sucssesfuly added to the Cart"});
				}
				else
				{
					return BadRequest(new { message = "Something went wrong while adding the product to the Cart" });
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return BadRequest();
			}
		}

		[HttpPost("update")]
		public async Task<IActionResult> UpdateCartItem([FromBody]UpdateCartItemDto model)
		{
			try
			{
				string? userId = this.GetUserId();
				int? quantity = model.Quantity;
				int? itemId = model.ItemId;

				ShoppingCartSummaryViewModel? summaryModel = await this._shoppingCartService
									.UpdateCartItemAsync(userId, quantity, itemId);

				if (summaryModel != null)
				{
					return Ok(new {
						itemTotalPrice = summaryModel.ItemTotalPrice,
						subTotal = summaryModel.SubTotal,
						shipping = summaryModel.Shipping,
						total = summaryModel.Total
					});
				}
				else
				{
					return BadRequest(new { message = "Something went wrong while updateing the cart item!" });
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				
				return BadRequest();
			}
		}

		[HttpPost("remove/{itemId}")]
		public async Task<IActionResult> RemoveCartItem(int? itemId)
		{
			try
			{
				string? userId = this.GetUserId();

				ShoppingCartSummaryViewModel? summaryModel = await this._shoppingCartService
									.RemoveCartItemAsync(userId, itemId);

				if (summaryModel != null)
				{
					return Ok(new {
						subTotal = summaryModel.SubTotal,
						shipping = summaryModel.Shipping,
						total = summaryModel.Total
					});
				}
				else
				{
					return BadRequest(new { message = "Something went wrong while removing the cart item!" });
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return BadRequest();
			}
		}

		[AllowAnonymous]
		[HttpGet("count")]
		public async Task<IActionResult> GetCartItemsCount()
		{
			try
			{
				string? userId = this.GetUserId();

				int cartItemsCount;
				if (userId != null)
				{
					cartItemsCount = await this._shoppingCartService
								.GetUserShoppingCartItemsCountAsync(userId);
				}
				else
				{
					string? guestId = this.HttpContext.Items["GuestIdentifier"]?.ToString();
					cartItemsCount = await this._shoppingCartService
								.GetGuestShoppingCartItemsCountAsync(guestId);
				}

				return Ok(new
				{
					count = cartItemsCount
				});
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return BadRequest("Something went wrong!");
			}
		}
	}
}

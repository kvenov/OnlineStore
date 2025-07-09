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

		[HttpPost("add/{productId}")]
		public async Task<IActionResult> AddToCart(int? productId)
		{

			try
			{
				string? userId = this.GetUserId();

				bool isAdded = await this._shoppingCartService
							.AddToCartAsync(productId, userId);

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
	}
}

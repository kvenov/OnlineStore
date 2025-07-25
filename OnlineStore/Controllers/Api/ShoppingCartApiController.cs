﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Services.Core.DTO.ShoppingCart;
using OnlineStore.Web.ViewModels.ShoppingCart;
using OnlineStore.Web.ViewModels.Product;

namespace OnlineStore.Web.Controllers.Api
{
	[Route("api/[controller]")]
	[ApiController]
	public class ShoppingCartApiController : BaseApiController
	{
		private readonly IShoppingCartService _shoppingCartService;
		private readonly IProductService _productService;

		public ShoppingCartApiController(IShoppingCartService shoppingCartService, IProductService productService)
		{
			this._shoppingCartService = shoppingCartService;
			this._productService = productService;
		}

		[AllowAnonymous]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[HttpPost("add/{productId}/{productSize}")]
		public async Task<IActionResult> AddToCart(int? productId, string? productSize)
		{

			try
			{
				string? userId = this.GetUserId();

				bool isAdded;
				if (userId != null)
				{
					isAdded = await this._shoppingCartService
							.AddToCartForUserAsync(productId, productSize, userId);
				}
				else
				{
					string? guestId = this.GetGuestId();
					isAdded = await this._shoppingCartService
							.AddToCartForGuestAsync(productId, productSize, guestId);
				}

				AllProductListViewModel? product = await this._productService
							.GetProductByIdAsync(productId);

				if ((isAdded) && (product != null))
				{
					return Ok(new { product });
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

		[AllowAnonymous]
		[HttpPost("update")]
		public async Task<IActionResult> UpdateCartItem([FromBody]UpdateCartItemDto model)
		{
			try
			{
				string? userId = this.GetUserId();
				int? quantity = model.Quantity;
				int? itemId = model.ItemId;


				ShoppingCartSummaryViewModel? summaryModel;
				if (userId != null)
				{
					summaryModel = await this._shoppingCartService
									.UpdateUserCartItemAsync(userId, quantity, itemId);
				}
				else
				{
					string? guestId = this.GetGuestId();

					summaryModel = await this._shoppingCartService
									.UpdateGuestCartItemAsync(guestId, quantity, itemId);
				}

				if (summaryModel != null)
				{
					return Ok(new
					{
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

		[AllowAnonymous]
		[HttpPost("remove/{itemId}")]
		public async Task<IActionResult> RemoveCartItem(int? itemId)
		{
			try
			{
				string? userId = this.GetUserId();

				ShoppingCartSummaryViewModel? summaryModel;

				if (userId != null)
				{
					summaryModel = await this._shoppingCartService
									.RemoveUserCartItemAsync(userId, itemId);
				}
				else
				{
					string? guestId = this.GetGuestId();

					summaryModel = await this._shoppingCartService
									.RemoveGuestCartItemAsync(guestId, itemId);
				}

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
					string? guestId = this.GetGuestId();
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

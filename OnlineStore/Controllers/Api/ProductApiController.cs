using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Core.DTO.Product;
using OnlineStore.Services.Core.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Web.Controllers.Api
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductApiController : BaseApiController
	{
		private readonly IProductService _productService;

		public ProductApiController(IProductService productService)
		{
			this._productService = productService;
		}

		[HttpGet("search")]
		[AllowAnonymous]
		public async Task<IActionResult> GetProducts(string? query)
		{
			try
			{
				IEnumerable<GetSearchedProductsDto> products = await this._productService
											.GetSearchedProductsAsync(query, maxResults: 5);
				if (products == null)
				{
					return NotFound(new { Message = "No products found." });
				}

				var searchedProducts = products.Select(p => new
				{
					id = p.Id,
					name = p.Name,
					price = p.Price,
					imageUrl = p.ImageUrl
				});

				return Ok(searchedProducts);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return StatusCode(StatusCodes.Status500InternalServerError,
							new { Message = "An error occurred while processing your request.", Error = ex.Message });
			}
		}

		[HttpGet("sizes/{productId}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[AllowAnonymous]
		public async Task<ActionResult> GetProductSizes([Required]int? productId)
		{
			try
			{
				IEnumerable<string>? sizes = await this._productService
							.GetProductSizesAsync(productId);

				if (sizes == null || !sizes.Any())
				{
					return NotFound(new { Message = "No sizes found." });
				}

				return Ok(sizes);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return StatusCode(StatusCodes.Status500InternalServerError, 
							new { Message = "An error occurred while processing your request.", Error = ex.Message });
			}
		}
	}
}

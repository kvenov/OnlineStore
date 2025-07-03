using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Core.Admin.Interfaces;
using OnlineStore.Web.ViewModels.Admin.ProductPromotion;

namespace OnlineStore.Web.Areas.Admin.Controllers.Api
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	[Route("api/[controller]")]
	[ApiController]
	public class ProductApiController : ControllerBase
	{
		private readonly IAdminProductService _productService;

		public ProductApiController(IAdminProductService productService)
		{
			this._productService = productService;
		}

		[HttpGet("get")]
		public async Task<IActionResult> GetProducts()
		{
			try
			{
				IEnumerable<PromotionProductViewModel> products = await this._productService
							.GetProductsIdsAndNamesAsync();

				if (products == null)
				{
					return BadRequest();
				}

				return Ok(products);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.InnerException);
				
				return BadRequest();
			}
		}
	}
}

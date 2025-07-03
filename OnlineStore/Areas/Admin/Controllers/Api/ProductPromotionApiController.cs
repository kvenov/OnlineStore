using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Core.Admin.Interfaces;
using OnlineStore.Web.ViewModels.Admin.ProductPromotion;

namespace OnlineStore.Web.Areas.Admin.Controllers.Api
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductPromotionApiController : ControllerBase
	{
		private readonly IAdminProductPromotionService _productPromotionService;

		public ProductPromotionApiController(IAdminProductPromotionService productPromotionService)
		{
			this._productPromotionService = productPromotionService;
		}

		[HttpPost("create")]
		public async Task<IActionResult> Create([FromBody]AddPromotionInputModel model)
		{

			try
			{
				if (!ModelState.IsValid)
				{
					var errorMessages = ModelState.Values
							.SelectMany(v => v.Errors)
							.Select(e => e.ErrorMessage);

					return BadRequest(new { message = "Validation failed", errors = errorMessages });
				}

				bool result = await this._productPromotionService
							.CreateProductPromotion(model);

				if (result)
				{
					return Ok();
				}
				else
				{
					return BadRequest();
				}
			}
			catch (Exception ex)
			{

				Console.WriteLine(ex.Message);

				return BadRequest(new { message = "Something went wrong"});
			}
		}
	}
}

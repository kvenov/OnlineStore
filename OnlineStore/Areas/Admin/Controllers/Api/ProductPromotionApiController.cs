using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Core.Admin.Interfaces;
using OnlineStore.Web.ViewModels.Admin.ProductPromotion;

namespace OnlineStore.Web.Areas.Admin.Controllers.Api
{

	[AutoValidateAntiforgeryToken]
	public class ProductPromotionApiController : BaseAdminApiController
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


		[HttpGet("get/{promotionId}")]
		public async Task<IActionResult> Get(int promotionId)
		{
			try
			{
				PromotionGetViewModel? model = await this._productPromotionService
									.GetPromotionByIdAsync(promotionId);

				if (model == null)
				{
					return BadRequest(new { error = "Something went wrong while getting the promotion" });
				}


				return Ok(model);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return BadRequest(new {message = "Something went wrong"});
			}
		}

		[HttpPost("edit")]
		public async Task<IActionResult> Edit([FromBody]EditPromotionInputModel? model)
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

				bool isEdited = await this._productPromotionService
							.EditPromotionAsync(model);

				if (isEdited)
				{
					return Ok();
				}
				else
				{
					return BadRequest("The process of the promotion edit has failed!");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return BadRequest(new { message = "Something went wrong" });
			}
		}

		[HttpPost("delete/{promotionId}")]
		public async Task<IActionResult> Delete(int? promotionId)
		{
			try
			{
				bool isDeleted = await this._productPromotionService
							.DeletePromotionAsync(promotionId);

				if (isDeleted)
				{
					return Ok();
				}
				else
				{
					return BadRequest("The process of the promotion deletion has failed!");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return BadRequest(new { message = "Something went wrong" });
			}
		}
	}
}

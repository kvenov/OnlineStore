using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Core.Admin.Interfaces;
using OnlineStore.Web.ViewModels.Admin.ProductPromotion;

namespace OnlineStore.Web.Areas.Admin.Controllers
{

	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class ProductPromotionController : Controller
	{
		private readonly IAdminProductPromotionService _productPromotionService;

		public ProductPromotionController(IAdminProductPromotionService productPromotionService)
		{
			this._productPromotionService = productPromotionService;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{

			try
			{
				IEnumerable<PromotionIndexViewModel> promotions = await this._productPromotionService
							.GetProductsPromotionsAsync();

				return View(promotions);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return this.RedirectToAction("Index", "Home");
			}
		}
	}
}

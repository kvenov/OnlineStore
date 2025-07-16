using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.Product;

namespace OnlineStore.Web.Controllers
{
	public class ProductController : BaseController
	{
		private readonly IProductService _productService;

		public ProductController(IProductService service)
		{
			this._productService = service;
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> ProductList()
		{
			try
			{
				IEnumerable<AllProductListViewModel> productList = await this._productService
						.GetAllProductsAsync();

				if (productList == null)
				{
					return RedirectToAction("Index", "Home");
				}

				return View(productList);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return RedirectToAction("Index", "Home");
			}
		}

		[HttpGet]
		[Route("products/{gender}/{category}/{subcategory?}")]
		[AllowAnonymous]
		[ActionName("List")]
		public async Task<IActionResult> GetFilteredProducts(string? gender, string? category, string? subCategory)
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(category) && !string.IsNullOrWhiteSpace(subCategory))
				{
					category = category.Replace("@", "/");
					subCategory = subCategory.Replace("@", "/");
				}

				IEnumerable<AllProductListViewModel> filteredProducts = await this._productService
												.GetFilteredProductsAsync(gender, category, subCategory);

				ViewBag.Gender = gender;
				ViewBag.Category = category;
				ViewBag.SubCategory = subCategory;

				return View(filteredProducts);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				
				return RedirectToAction("Index", "Home");
			}
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Details(int? id)
		{
			try
			{
				string? userId = this.GetUserId();

				ProductDetailsViewModel? productDetails = await this._productService
							.GetProductDetailsByIdAsync(id, userId);

				if (productDetails == null)
				{
					return NotFound();
				}

				return View(productDetails);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return RedirectToAction("Index", "Home");
			}
		}

		[HttpPost]
		public async Task<IActionResult> AddReview([FromForm]int? id, [FromForm] int? rating, [FromForm]string? content)
		{
			try
			{
				string userId = this.GetUserId()!;

				bool isAdded = await this._productService
						.AddProductReviewAsync(id, rating, content, userId);

				if (isAdded)
				{
					return this.RedirectToAction(nameof(Details), new { id });
				}
				else
				{
					return this.RedirectToAction(nameof(Details), new { id });
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.InnerException);

				return RedirectToAction("Index", "Home");
			}
		}

		[HttpPost]
		public async Task<IActionResult> EditReview([FromForm]int? reviewId, [FromForm]int id, 
					[FromForm]int? rating, [FromForm]int? ratingId, [FromForm]string? content)
		{
			try
			{
				string userId = this.GetUserId()!;

				bool isEdited = await this._productService
									.EditProductReviewAsync(reviewId, rating, ratingId, content, userId);

				if (isEdited)
				{
					return this.RedirectToAction(nameof(Details), new { id });
				}
				else
				{
					return this.RedirectToAction(nameof(Details), new { id });
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return this.RedirectToAction("Index", "Home");
			}
		}

		[HttpPost]
		public async Task<IActionResult> RemoveReview([FromForm]int? reviewId, [FromForm]int? ratingId, [FromForm]int productId)
		{
			try
			{
				string userId = this.GetUserId()!;

				bool isRemoved = await this._productService
									.RemoveProductReviewAsync(reviewId, ratingId, userId);

				if (isRemoved)
				{
					return this.RedirectToAction(nameof(Details), new { id = productId });
				}
				else
				{
					return this.RedirectToAction(nameof(Details), new { id = productId });
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return this.RedirectToAction("Index", "Home");
			}
		}
	}
}

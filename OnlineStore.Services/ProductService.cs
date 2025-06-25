using OnlineStore.Data;
using OnlineStore.Services.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Web.ViewModels.Product;
using OnlineStore.Web.ViewModels.Product.Partial;
using OnlineStore.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace OnlineStore.Services.Core
{
	public class ProductService : IProductService
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public ProductService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			this._context = context;
			this._userManager = userManager;
		}

		public async Task<IEnumerable<AllProductListViewModel>> GetAllProductsAsync()
		{
			IEnumerable<AllProductListViewModel> productList = await _context
			  .Products
			  .AsNoTracking()
			  .Select(p => new AllProductListViewModel
			  {
				  Id = p.Id,
				  Name = p.Name,
				  Description = p.Description,
				  Price = p.Price,
				  ImageUrl = p.ImageUrl,
				  Rating  = (float)p.AverageRating
			  })
			  .ToListAsync();

			return productList;
		}

		public async Task<AllProductListViewModel> GetProductByIdAsync(int id)
		{
			return await _context
				.Products
				.AsNoTracking()
				.Select(p => new AllProductListViewModel
				{
					Id = p.Id,
					Name = p.Name,
					Description = p.Description,
					Price = p.Price,
					ImageUrl = p.ImageUrl
				})
				.FirstAsync(p => p.Id == id);
		}

		public async Task<ProductDetailsViewModel?> GetProductDetailsByIdAsync(int? productId, string? userId)
		{
			ProductDetailsViewModel? productDetails = null;

			if (productId != null)
			{

				productDetails = await _context
					.Products
					.AsNoTracking()
					.Include(p => p.Category)
					.Include(p => p.Brand)
					.Include(p => p.ProductDetails)
					.Where(p => p.Id == productId)
					.Select(p => new ProductDetailsViewModel
					{
						Id = p.Id,
						Name = p.Name,
						Description = p.Description,
						Price = p.Price.ToString("C"),
						DiscountPrice = p.DiscountPrice.ToString(),
						ImageUrl = p.ImageUrl,
						AverageRating = p.AverageRating.ToString("0.0"),
						Category = p.Category.Name,
						Brand = p.Brand!.Name,
						IsProductReviewed = userId != null && p.ProductReviews.Any(pr => pr.UserId == userId),
						AvailableSizes = GetSizesForCategory(p.Category.Name),
						Details = new ProductDetailsPartialViewModel
						{
							Material = p.ProductDetails.Material,
							Color = p.ProductDetails.Color,
							Gender = p.ProductDetails.Gender,
							SizeGuideUrl = p.ProductDetails.SizeGuideUrl,
							CountryOfOrigin = p.ProductDetails.CountryOfOrigin,
							CareInstructions = p.ProductDetails.CareInstructions,
							Weight = p.ProductDetails.Weight,
							Fit = p.ProductDetails.Fit,
							Style = p.ProductDetails.Style
						},
						Reviews = p.ProductReviews
								  .Select(r => new ProductReviewPartialViewModel
								  {
									  Id = r.Id,
									  PublisherId = r.UserId,
									  Publisher = r.User.UserName,
									  Content = r.Content
								  })
					})
					.SingleOrDefaultAsync();
			}

			return productDetails;
		}

		public async Task<bool> AddProductReviewAsync(int? productId, int? rating, string? content, string userId)
		{
			bool isAdded = false;

			if ((productId != null) && (rating != null) && (content != null))
			{
				Product? product = await this._context
					.Products
					.FindAsync(productId);

				ApplicationUser? user = await this._userManager
					.FindByIdAsync(userId);

				bool isValidRating = (rating > 0) && (rating <= 5);

				ProductReview? existingProductReview = await this._context
					.ProductReviews
					.SingleOrDefaultAsync(pr => pr.ProductId == productId && pr.UserId == userId);

				ProductRating? existingProductRating = await this._context
					.ProductsRatings
					.SingleOrDefaultAsync(pr => pr.ProductId == productId && pr.UserId == userId);


				if ((product != null) && (user != null) && 
						(isValidRating) && (existingProductReview == null) 
										&& (existingProductRating == null))
				{
					ProductReview productReview = new ProductReview()
					{
						ProductId = product.Id,
						UserId = user.Id,
						Content = content
					};

					ProductRating productRating = new ProductRating()
					{
						ProductId = product.Id,
						UserId = user.Id,
						Rating = (int)rating,
						IsDeleted = false
					};

					await this._context.ProductReviews.AddAsync(productReview);
					await this._context.ProductsRatings.AddAsync(productRating);

					await this._context.SaveChangesAsync();
					isAdded = true;
				}
			}

			return isAdded;
		}



		private static IEnumerable<string> GetSizesForCategory(string categoryName)
		{
			return categoryName.Trim().ToLower() switch
			{
				"shoes" => new List<string>() { "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47" },
				"t-shirts" => new List<string>() { "XS", "S", "M", "L", "XL", "XXL" },
				"jeans" => new List<string>() { "28", "30", "32", "34", "36", "38", "40" },
				"jackets" => new List<string>() { "XS", "S", "M", "L", "XL", "XXL" },
				"hoodies" or "sweatshirts" => new List<string>() { "XS", "S", "M", "L", "XL", "XXL" },
				"shorts" => new List<string>() { "28", "30", "32", "34", "36", "38" },
				"socks" => new List<string>() { "36-38", "39-42", "43-46" },
				"underwear" => new List<string>() { "XS", "S", "M", "L", "XL" },
				_ => new List<string>() { "One Size" }
			};
		}
	}
}

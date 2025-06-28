using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.Data.Models;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.Product;
using OnlineStore.Web.ViewModels.Product.Partial;

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
						IsProductReviewed = userId != null && p.ProductReviews.Where(pr => pr.IsDeleted == false)
																.Any(pr => pr.UserId == userId) && 
															  p.ProductRatings.Where(pr => pr.IsDeleted == false)
															    .Any(pr => pr.UserId == userId),
						AvailableSizes = GetSizesForCategory(p.Category.Name),
						Details = new ProductDetailsPartialViewModel()
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
								  .Where(r => r.IsDeleted == false)
								  .Select(r => new ProductReviewPartialViewModel()
								  {
									  Id = r.Id,
									  PublisherId = r.UserId,
									  Publisher = r.User.UserName,
									  Content = r.Content
								  }),
						Ratings = p.ProductRatings
								  .Where(r => r.IsDeleted == false)
								  .Select(r => new ProductRatingPartialViewModel()
								  {
									  Id = r.Id,
									  UserId = r.UserId,
									  Rating = r.Rating
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

				bool isRatingValid = (rating > 0) && (rating <= 5);

				bool isContentValid = !string.IsNullOrWhiteSpace(content);

				ProductReview? existingProductReview = await this._context
					.ProductReviews
					.IgnoreQueryFilters()
					.SingleOrDefaultAsync(pr => pr.ProductId == productId && pr.UserId == userId);

				ProductRating? existingProductRating = await this._context
					.ProductsRatings
					.IgnoreQueryFilters()
					.SingleOrDefaultAsync(pr => pr.ProductId == productId && pr.UserId == userId);


				if ((product != null) && (user != null) && 
						(isRatingValid) && (isContentValid))
				{
					if ((existingProductReview == null) && (existingProductRating == null))
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
							Rating = rating.Value,
							IsDeleted = false
						};

						await this._context.ProductReviews.AddAsync(productReview);
						await this._context.ProductsRatings.AddAsync(productRating);
					}
					else
					{
						if (existingProductReview != null)
						{
							existingProductReview.IsDeleted = false;
							existingProductReview.Content = content;
							this._context.ProductReviews.Update(existingProductReview);
						}

						if (existingProductRating != null)
						{
							existingProductRating.IsDeleted = false;
							existingProductRating.Rating = rating.Value;
							this._context.ProductsRatings.Update(existingProductRating);
						}
					}


					await this._context.SaveChangesAsync();
					isAdded = true;
				}
			}

			return isAdded;
		}

		public async Task<bool> EditProductReviewAsync(int? reviewId, int? rating, int? ratingId, string? content, string userId)
		{
			bool isEdited = false;

			if ((reviewId != null) && (rating != null) && (ratingId != null) && (content != null))
			{
				ProductReview? review = await this._context
					.ProductReviews
					.FindAsync(reviewId);

				ProductRating? productRating = await this._context
					.ProductsRatings
					.FindAsync(ratingId);

				ApplicationUser? user = await this._userManager
					.FindByIdAsync(userId);

				bool isValidRating = (rating > 0) && (rating <= 5);

				if ((review != null) && (productRating != null) && (user != null) 
					&& (review.UserId == userId) && (productRating.UserId == userId) && (isValidRating))
				{
					review.Content = content;
					productRating.Rating = rating.Value;

					await this._context.SaveChangesAsync();
					isEdited = true;
				}
			}

			return isEdited;
		}
		public async Task<bool> RemoveProductReviewAsync(int? reviewId, int? ratingId, string userId)
		{
			bool isRemoved = false;

			if ((reviewId != null) && (ratingId != null))
			{
				ProductReview? review = await this._context
					.ProductReviews
					.FindAsync(reviewId);

				ProductRating? productRating = await this._context
					.ProductsRatings
					.FindAsync(ratingId);

				ApplicationUser? user = await this._userManager
					.FindByIdAsync(userId);

				if ((review != null) && (productRating != null) && (user != null)
					 && (review.UserId == user.Id) && (productRating.UserId == user.Id))
				{

					this._context.ProductReviews.Remove(review);
					this._context.ProductsRatings.Remove(productRating);

					await this._context.SaveChangesAsync();

					isRemoved = true;
				}
			}

			return isRemoved;
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

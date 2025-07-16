using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Services.Core.DTO.Product;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.Home.Partial;
using OnlineStore.Web.ViewModels.Product;
using OnlineStore.Web.ViewModels.Product.Partial;

using static OnlineStore.Data.Common.Constants.EntityConstants.ProductDetails;

namespace OnlineStore.Services.Core
{
	public class ProductService : IProductService
	{
		private readonly IProductRepository _repository;
		private readonly IRepository<ProductCategory, int> _categoryRepository;
		private readonly UserManager<ApplicationUser> _userManager;

		public ProductService(IProductRepository repository, 
							  IRepository<ProductCategory, int> categoryRepository, 
							  UserManager<ApplicationUser> userManager)
		{
			this._repository = repository;
			this._userManager = userManager;
			this._categoryRepository = categoryRepository;
		}

		public async Task<IEnumerable<AllProductListViewModel>> GetAllProductsAsync()
		{
			IEnumerable<AllProductListViewModel> productList = await this._repository
			  .GetAllAttached()
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
			return await this._repository
				.GetAllAttached()
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

				productDetails = await this._repository
					.GetAllAttached()
					.AsNoTracking()
					.Include(p => p.Category)
					.Include(p => p.Brand)
					.Include(p => p.ProductDetails)
					.Include(p => p.ProductReviews)
					.Include(p => p.ProductRatings)
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
				Product? product = await this._repository
							.GetByIdAsync(productId.Value);

				ApplicationUser? user = await this._userManager
					.FindByIdAsync(userId);

				bool isRatingValid = (rating > 0) && (rating <= 5);

				bool isContentValid = !string.IsNullOrWhiteSpace(content);

				ProductReview? existingProductReview = await this._repository
					.GetAllReviewsAttached()
					.IgnoreQueryFilters()
					.SingleOrDefaultAsync(pr => pr.ProductId == productId && pr.UserId == userId);

				ProductRating? existingProductRating = await this._repository
					.GetAllRatingsAttached()
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

						await this._repository.AddProductReviewAsync(productReview);
						await this._repository.AddProductRatingAsync(productRating);
					}
					else
					{
						if (existingProductReview != null)
						{
							existingProductReview.IsDeleted = false;
							existingProductReview.Content = content;
						}

						if (existingProductRating != null)
						{
							existingProductRating.IsDeleted = false;
							existingProductRating.Rating = rating.Value;
						}
					}

					await this._repository.SaveChangesAsync();
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
				ProductReview? review = await this._repository
					.GetProductReviewByIdAsync(reviewId);

				ProductRating? productRating = await this._repository
					.GetProductRatingByIdAsync(ratingId);

				ApplicationUser? user = await this._userManager
					.FindByIdAsync(userId);

				bool isValidRating = (rating > 0) && (rating <= 5);

				if ((review != null) && (productRating != null) && (user != null) 
					&& (review.UserId == userId) && (productRating.UserId == userId) && (isValidRating))
				{
					review.Content = content;
					productRating.Rating = rating.Value;

					await this._repository.SaveChangesAsync();
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
				ProductReview? review = await this._repository
					.GetProductReviewByIdAsync(reviewId);

				ProductRating? productRating = await this._repository
					.GetProductRatingByIdAsync(ratingId);

				ApplicationUser? user = await this._userManager
					.FindByIdAsync(userId);

				if ((review != null) && (productRating != null) && (user != null)
					 && (review.UserId == user.Id) && (productRating.UserId == user.Id))
				{

					bool isReviewSucceed = await this._repository.DeleteReviewAsync(review);
					bool isRatingSucceed = await this._repository.DeleteRatingAsync(productRating);

					isRemoved = (isReviewSucceed) && (isRatingSucceed);
				}
			}

			return isRemoved;
		}

		public async Task<IEnumerable<TrendingProductViewModel>> GetBestProductsAsync()
		{
			IEnumerable<TrendingProductViewModel> trendings = await this._repository
						.GetAllAttached()
						.AsNoTracking()
						.Where(p => p.IsActive && p.StockQuantity > 0)
						.OrderByDescending(p =>
								(p.AverageRating * 3) +
								 p.OrderItems.Sum(oi => oi.Quantity))
						.Select(p => new TrendingProductViewModel()
						{
							ProductId = p.Id,
							ImageUrl = p.ImageUrl,
							ProductName = p.Name,
							Price = p.Price.ToString("F2")
						})
						.Take(6)
						.ToListAsync();

			return trendings;

		}

		public async Task<IEnumerable<AllProductListViewModel>> GetFilteredProductsAsync(string? gender, string? category, string? subCategory)
		{
			IEnumerable<AllProductListViewModel>? filteredProducts = new List<AllProductListViewModel>();

			if (!string.IsNullOrWhiteSpace(gender) && 
					!string.IsNullOrWhiteSpace(category) && 
						!string.IsNullOrWhiteSpace(subCategory))
			{
				bool isGenderValid = AllowedGenders
						.Any(g => g.ToLower() == gender.ToLower());

				bool isCategoryValid = await this._categoryRepository
						.GetAllAttached()
						.AsNoTracking()
						.Where(c => c.ParentCategoryId == null)
						.AnyAsync(c => c.Name.ToLower() == category.ToLower());

				bool isSubCategoryValid = await this._categoryRepository
						.GetAllAttached()
						.AsNoTracking()
						.Where(c => c.ParentCategoryId != null)
						.AnyAsync(c => c.Name.ToLower() == subCategory.ToLower());

				if (isGenderValid && isCategoryValid && isSubCategoryValid)
				{
					filteredProducts = await this._repository
						.GetAllAttached()
						.AsNoTracking()
						.Include(p => p.Category)
						.ThenInclude(c => c.ParentCategory)
						.Include(p => p.ProductDetails)
						.Where(p => p.ProductDetails.Gender.ToLower() == gender.ToLower())
						.Where(p => p.Category.Name.ToLower() == subCategory.ToLower() &&
									p.Category.ParentCategory!.Name.ToLower() == category.ToLower())
						.Select(p => new AllProductListViewModel()
						{
							Id = p.Id,
							Name = p.Name,
							Description = p.Description,
							Price = p.Price,
							ImageUrl = p.ImageUrl,
							Rating = (float)p.AverageRating
						})
						.ToListAsync();
				}
			}

			return filteredProducts;
		}

		public async Task<IEnumerable<GetSearchedProductsDto>> GetSearchedProductsAsync(string? query, int maxResults = 5)
		{
			IEnumerable<GetSearchedProductsDto> searchedProducts = new List<GetSearchedProductsDto>();
			if (!string.IsNullOrWhiteSpace(query))
			{
				searchedProducts = await this._repository
					.GetAllAttached()
					.AsNoTracking()
					.Where(p => p.Name.ToLower().Contains(query.Trim().ToLower()) ||
								p.Description.ToLower().Contains(query.Trim().ToLower()))
					.Select(p => new GetSearchedProductsDto
					{
						Id = p.Id,
						Name = p.Name,
						ImageUrl = p.ImageUrl,
						Price = p.Price.ToString("C")
					})
					.Take(maxResults)
					.ToListAsync();
			}

			return searchedProducts;
		}

		private static IEnumerable<string> GetSizesForCategory(string categoryName)
		{
			return categoryName.Trim().ToLower() switch
			{
				"sporty shoes" => new List<string>() { "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47" },
				"lifestyle shoes" => new List<string>() { "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47" },
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

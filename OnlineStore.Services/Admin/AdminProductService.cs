using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Services.Core.Admin.Interfaces;
using OnlineStore.Web.ViewModels.Admin.Product;
using OnlineStore.Web.ViewModels.Admin.ProductPromotion;
using static OnlineStore.Data.Common.Constants.EntityConstants.ProductDetails;

namespace OnlineStore.Services.Core.Admin
{
	public class AdminProductService : IAdminProductService
	{
		private readonly IProductRepository _repository;

		//TODO: Think of a better idea to use this repositories!!!
		private readonly IAsyncRepository<ProductCategory, int> _asyncCategoryRepository;
		private readonly IAsyncRepository<Brand, int> _asyncBrandRepository;
		private readonly IRepository<ProductCategory, int> _categoryRepository;
		private readonly IRepository<Brand, int> _brandRepository;

		public AdminProductService(IProductRepository repository,
								  IAsyncRepository<ProductCategory, int> asyncCategoryRepository,
								  IAsyncRepository<Brand, int> asyncBrandRepository,
								  IRepository<ProductCategory, int> categoryRepository,
								  IRepository<Brand, int> brandRepository)
		{
			this._repository = repository;
			this._asyncCategoryRepository = asyncCategoryRepository;
			this._asyncBrandRepository = asyncBrandRepository;
			this._categoryRepository = categoryRepository;
			this._brandRepository = brandRepository;
		}

		public async Task<bool> AddProductAsync(AddProductInputModel model)
		{
			bool isAdded = false;
			ProductCategory? productCategory = await this._categoryRepository
				.GetAllAttached()
				.Include(pc => pc.Products)
				.FirstOrDefaultAsync(pc => pc.Id == model.CategoryId);

			if (productCategory == null)
				return false;

			Brand? brand = null;
			if (model.BrandId.HasValue)
			{
				brand = await this._brandRepository
					.GetAllAttached()
					.Include(b => b.Products)
					.FirstOrDefaultAsync(b => b.Id == model.BrandId.Value);
			}

			bool isAvgRatingValid = double.TryParse(model.AverageRating, out var avgRating);

			if ((productCategory != null) && (isAvgRatingValid))
			{
				Product product = new Product
				{
					Name = model.Name,
					Description = model.Description,
					ImageUrl = model.ImageUrl,
					Price = model.Price,
					DiscountPrice = model.DiscountPrice,
					IsActive = model.IsActive,
					StockQuantity = model.StockQuantity,
					AverageRating = avgRating,
					TotalRatings = model.TotalRatings,
					CategoryId = productCategory.Id,
					BrandId = brand == null ? null : brand.Id
				};

				ProductDetails productDetails = new ProductDetails()
				{
					Material = model.Material,
					Color = model.Color,
					Gender = model.Gender,
					SizeGuideUrl = model.SizeGuideUrl,
					CountryOfOrigin = model.CountryOfOrigin,
					CareInstructions = model.CareInstructions,
					Weight = model.Weight,
					Fit = model.Fit,
					Style = model.Style
				};

				product.ProductDetails = productDetails;
				productCategory.Products.Add(product);
				if (brand != null)
				{
					brand.Products.Add(product);
				}

				await this._repository.AddAsync(product);

				isAdded = true;
			}

			return isAdded;
		}

		public async Task<bool> SoftDeleteProductAsync(string? id)
		{
			bool isRemoved = false;

			if (!string.IsNullOrWhiteSpace(id))
			{
				Product? productToDelete = await this._repository
					.SingleOrDefaultAsync(p => p.Id.ToString().ToLower() == id!.ToLower());

				if (productToDelete != null)
				{

					//This method will set the IsDeleted prop to true and SaveChanges: All Async!
					isRemoved = await this._repository.DeleteAsync(productToDelete);
				}
			}

			return isRemoved;
		}

		public async Task<IEnumerable<AllProductsViewModel>> GetAllProductsAsync()
		{
			IEnumerable<AllProductsViewModel> productList = await this._repository
				.GetAllAttached()
				.Include(p => p.Brand)
				.Include(p => p.Category)
				.AsNoTracking()
				.Select(p => new AllProductsViewModel
				{
					Id = p.Id.ToString(),
					Name = p.Name,
					Description = p.Description,
					Price = p.Price.ToString("F2"),
					DiscountPrice = p.DiscountPrice.HasValue ? p.DiscountPrice.Value.ToString("F2") : null,
					Category = p.Category.Name,
					Brand = p.Brand != null ? p.Brand.Name : "",
					ImageUrl = p.ImageUrl
				})
				.ToListAsync();

			return productList;
		}

		public async Task<ProductDetailsForDeleteViewModel?> GetProductDetailsForDeleteAsync(string id)
		{
			ProductDetailsForDeleteViewModel? productDetails = null;
			bool isValidId = int.TryParse(id, out int productId);

			if (isValidId)
			{
				productDetails = await this._repository
					.GetAllAttached()
					.AsNoTracking()
					.Where(p => p.Id == productId)
					.Select(p => new ProductDetailsForDeleteViewModel
					{
						Id = p.Id.ToString(),
						Name = p.Name,
						Description = p.Description,
						ImageUrl = p.ImageUrl,
						Price = p.Price.ToString("F2"),
						Category = p.Category.Name,
						Brand = p.Brand != null ? p.Brand.Name : "",
					})
					.SingleOrDefaultAsync();
			}

			return productDetails;
		}

		public async Task<EditProductInputModel?> GetEditableProductByIdAsync(int? id)
		{
			EditProductInputModel? model = null;

			if (id != null)
			{

				Product? product = await this._repository
					.GetAllAttached()
					.Include(p => p.Category)
					.Include(p => p.Brand)
					.Include(p => p.ProductDetails)
					.SingleOrDefaultAsync(p => p.Id == id);

				if (product != null)
				{
					model = new EditProductInputModel
					{
						Id = product.Id,
						Name = product.Name,
						Description = product.Description,
						ImageUrl = product.ImageUrl,
						Price = product.Price,
						DiscountPrice = product.DiscountPrice,
						IsActive = product.IsActive,
						StockQuantity = product.StockQuantity,
						AverageRating = product.AverageRating.ToString("F1"),
						TotalRatings = product.TotalRatings,
						CategoryId = product.CategoryId,
						BrandId = product.BrandId,
						Material = product.ProductDetails.Material,
						Color = product.ProductDetails.Color,
						Gender = product.ProductDetails.Gender,
						SizeGuideUrl = product.ProductDetails.SizeGuideUrl,
						CountryOfOrigin = product.ProductDetails.CountryOfOrigin,
						CareInstructions = product.ProductDetails.CareInstructions,
						Weight = product.ProductDetails.Weight,
						Fit = product.ProductDetails.Fit,
						Style = product.ProductDetails.Style,
					};

				}	
			}

			return model;
		}

		public async Task<bool> EditProductAsync(EditProductInputModel? model)
		{
			bool isEdited = false;

			if (model != null)
			{
				Product? product = await this._repository
					.GetAllAttached()
					.Include(p => p.ProductDetails)
					.Include(p => p.Category)
					.Include(p => p.Brand)
					.SingleOrDefaultAsync(p => p.Id == model.Id);

				ProductCategory? productCategory = await this._asyncCategoryRepository
					.SingleOrDefaultAsync(pc => pc.Id == model.CategoryId);

				Brand? brand = null;
				if (model.BrandId.HasValue)
				{
					brand = await this._asyncBrandRepository
						.SingleOrDefaultAsync(b => b.Id == model.BrandId.Value);
				}

				bool isAvgRatingValid = double.TryParse(model.AverageRating, out var avgRating);

				if ((product != null) && (productCategory != null) && (isAvgRatingValid))
				{
					if (product.Brand != null)
					{
						product.Brand.Products.Remove(product);
					}
					product.Category.Products.Remove(product);


					product.Name = model.Name;
					product.Description = model.Description;
					product.ImageUrl = model.ImageUrl;
					product.Price = model.Price;
					product.DiscountPrice = model.DiscountPrice;
					product.IsActive = model.IsActive;
					product.StockQuantity = model.StockQuantity;
					product.AverageRating = avgRating;
					product.TotalRatings = model.TotalRatings;
					product.CategoryId = productCategory.Id;
					product.Category = productCategory;
					product.BrandId = brand != null ? brand.Id : null;
					product.Brand = brand;
					product.ProductDetails.Material = model.Material;
					product.ProductDetails.Color = model.Color;
					product.ProductDetails.Gender = model.Gender;
					product.ProductDetails.SizeGuideUrl = model.SizeGuideUrl;
					product.ProductDetails.CountryOfOrigin = model.CountryOfOrigin;
					product.ProductDetails.CareInstructions = model.CareInstructions;
					product.ProductDetails.Weight = model.Weight;
					product.ProductDetails.Fit = model.Fit;
					product.ProductDetails.Style = model.Style;

					if (brand != null)
					{
						brand.Products.Add(product);
					}
					productCategory.Products.Add(product);

					isEdited = await this._repository.UpdateAsync(product);
				}
			}

			return isEdited;
		}

		public async Task<ProductDetailsViewModel?> GetProductDetailsByIdAsync(int? id)
		{
			ProductDetailsViewModel? productDetails = null;

			if (id != null)
			{
				Product? product = await this._repository
					.GetAllAttached()
					.Include(p => p.Category)
					.Include(p => p.Brand)
					.Include(p => p.ProductDetails)
					.AsNoTracking()
					.SingleOrDefaultAsync(p => p.Id == id);

				if (product != null)
				{

					productDetails = new ProductDetailsViewModel()
					{
						Id = product.Id,
						Name = product.Name,
						Description = product.Description,
						ImageUrl = product.ImageUrl,
						Price = product.Price,
						DiscountPrice = product.DiscountPrice,
						IsActive = product.IsActive,
						StockQuantity = product.StockQuantity,
						AverageRating = product.AverageRating,
						TotalRatings = product.TotalRatings,
						Category = product.Category.Name,
						Brand = product.Brand != null ? product.Brand.Name : null,
						Material = product.ProductDetails.Material,
						Color = product.ProductDetails.Color,
						Gender = product.ProductDetails.Gender,
						SizeGuideUrl = product.ProductDetails.SizeGuideUrl,
						CountryOfOrigin = product.ProductDetails.CountryOfOrigin,
						CareInstructions = product.ProductDetails.CareInstructions,
						Weight = product.ProductDetails.Weight,
						Fit = product.ProductDetails.Fit,
						Style = product.ProductDetails.Style
					};

				}
			}

			return productDetails;
		}

		public async Task<IEnumerable<PromotionProductViewModel>> GetProductsIdsAndNamesAsync()
		{
			IEnumerable<PromotionProductViewModel> products = await this._repository
				.GetAllAttached()
				.AsNoTracking()
				.Select(p => new PromotionProductViewModel()
				{
					Id = p.Id,
					Name = p.Name
				})
				.ToListAsync();

			return products;
		}

		public IEnumerable<SelectListItem> GetGendersForProductDetails()
		{
			List<SelectListItem> genders = new List<SelectListItem>();

			foreach (var gender in AllowedGenders)
			{
				genders.Add(new SelectListItem(){
					Value = gender,
					Text = gender
				});
			}

			return genders;
		}
	}
}

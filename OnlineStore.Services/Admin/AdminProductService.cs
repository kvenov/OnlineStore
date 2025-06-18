using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.Data.Models;
using OnlineStore.Services.Core.Admin.Interfaces;
using OnlineStore.Web.ViewModels.Admin.Product;

namespace OnlineStore.Services.Core.Admin
{
	public class AdminProductService : IAdminProductService
	{
		private readonly ApplicationDbContext _context;

		public AdminProductService(ApplicationDbContext context)
		{
			this._context = context;
		}

		public async Task<bool> AddProductAsync(AddProductInputModel model)
		{
			bool isAdded = false;
			ProductCategory productCategory = await this._context
				.ProductCategories
				.Include(pc => pc.Products)
				.FirstAsync(pc => pc.Id == model.CategoryId);

			Brand? brand = null;
			if (model.BrandId.HasValue)
			{
				brand = await this._context
					.Brands
					.Include(b => b.Products)
					.FirstAsync(b => b.Id == model.BrandId.Value);
			}

			if (productCategory != null)
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
					AverageRating = model.AverageRating,
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

				await this._context.Products.AddAsync(product);
				await this._context.SaveChangesAsync();

				isAdded = true;
			}

			return isAdded;
		}

		public async Task<bool> SoftDeleteProductAsync(string? id)
		{
			bool isRemoved = false;

			if (string.IsNullOrWhiteSpace(id))
			{
				Product? productToDelete = await this._context
				.Products
				.SingleOrDefaultAsync(p => p.Id.ToString() == id);

				if (productToDelete != null)
				{

					productToDelete.IsDeleted = true;
					await this._context.SaveChangesAsync();

					isRemoved = true;
				}
			}

			return isRemoved;
		}

		public async Task<IEnumerable<AllProductsViewModel>> GetAllProductsAsync()
		{
			IEnumerable<AllProductsViewModel> productList = await _context
				.Products
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
				productDetails = await _context
					.Products
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

				Product? product = await this._context
					.Products
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
						AverageRating = product.AverageRating,
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
				Product? product = this._context
					.Products
					.Include(p => p.ProductDetails)
					.Include(p => p.Category)
					.Include(p => p.Brand)
					.SingleOrDefault(p => p.Id == model.Id);

				ProductCategory? productCategory = await this._context
					.ProductCategories
					.SingleOrDefaultAsync(pc => pc.Id == model.CategoryId);

				Brand? brand = null;
				if (model.BrandId.HasValue)
				{
					brand = await this._context
						.Brands
						.SingleOrDefaultAsync(b => b.Id == model.BrandId.Value);
				}

				if ((product != null) && (productCategory != null))
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
					product.AverageRating = model.AverageRating;
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

					await this._context.SaveChangesAsync();

					isEdited = true;
				}
			}

			return isEdited;
		}
	}
}

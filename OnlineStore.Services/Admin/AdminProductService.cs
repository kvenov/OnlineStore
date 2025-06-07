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

		public async Task AddProductAsync(AddProductViewModel model)
		{
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
		}

		public async Task DeleteProductAsync(string id)
		{
			Product? productToDelete = await this._context
				.Products
				.SingleOrDefaultAsync(p => p.Id.ToString() == id);

			if (productToDelete != null)
			{

				//Fix this to use async methods for the remove operation!!!
				this._context.Products.Remove(productToDelete);
				await this._context.SaveChangesAsync();
			}
		}

		public async Task<IEnumerable<AllProductsViewModel>> GetAllProductsAsync()
		{
			IEnumerable<AllProductsViewModel> productList = await _context
				.Products
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
	}
}

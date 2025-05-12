using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineStore.Data.Models;
using OnlineStore.Data.Seeding.Interfaces;
using System.Text.Json;

namespace OnlineStore.Data.Seeding
{
	public class ProductSeeder : BaseSeeder<ProductSeeder>, IEntitySeeder
	{
		private readonly ApplicationDbContext _context;

		public ProductSeeder(ApplicationDbContext context, ILogger<ProductSeeder> logger) : 
				base(logger)
		{
			this._context = context;
		}

		public override string FilePath =>
					Path.Combine(AppContext.BaseDirectory, "Files", "products.json");

		public async Task SeedEntityData()
		{
			await this.ImportProductsFromJson();
		}

		private async Task ImportProductsFromJson()
		{
			string productsJson = await File.ReadAllTextAsync(this.FilePath);

			List<Product> products = JsonSerializer.Deserialize<List<Product>>(productsJson, new JsonSerializerOptions()
			{
				PropertyNameCaseInsensitive = true,
			}) ?? new();

			var existingProductsNames = await this._context
					.Products
					.AsNoTracking()
					.Select(p => p.Name)
					.ToListAsync();

			var newProducts = products
					.Where(p => !existingProductsNames.Contains(p.Name))
					.ToList();

			var brandsIds = await this._context
					.Brands
					.AsNoTracking()
					.Select(b => b.Id)
					.ToListAsync();

			var productCategoriesIds = await this._context
					.ProductCategories
					.AsNoTracking()
					.Select(pc => pc.Id)
					.ToListAsync();

			List<Product> validProducts = new List<Product>();

			if (newProducts.Count > 0)
			{

				foreach (var product in newProducts)
				{

					if (product.BrandId != null)
					{
						if (!brandsIds.Contains(product.BrandId.Value))
						{
							
							this.Logger.LogWarning(
								$"Brand with id {product.BrandId} does not exist. Product {product.Name} will be added without brand.");
						}
					}

					if (!productCategoriesIds.Contains(product.CategoryId))
					{
						this.Logger.LogWarning(
							$"Product category with id {product.CategoryId} does not exist. Product {product.Name} will be added without category.");
					}

					validProducts.Add(product);
				}
			}
			else
			{
				this.Logger.LogWarning("No new products to seed.");
			}

			if (validProducts.Count > 0)
			{
				await this._context.Products.AddRangeAsync(validProducts);
				await this._context.SaveChangesAsync();
				this.Logger.LogInformation($"{validProducts.Count} products were added to the DB.");
			}
			else
			{
				this.Logger.LogWarning("No new products to seed.");
			}
		}
	}
}

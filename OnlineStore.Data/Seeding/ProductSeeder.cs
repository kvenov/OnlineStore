using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineStore.Data.Models;
using OnlineStore.Data.Seeding.Interfaces;
using System.Text.Json;
using static OnlineStore.Data.Common.OutputMessages.ErrorMessages;

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


			try
			{
				List<Product>? products = JsonSerializer.Deserialize<List<Product>>(productsJson, new JsonSerializerOptions()
				{
					PropertyNameCaseInsensitive = true
				});

				if (products != null && products.Count > 0)
				{
					ICollection<Product> validProducts = new List<Product>();

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

					if (newProducts.Count > 0)
					{

						foreach (var product in newProducts)
						{

							if (product.BrandId != null)
							{
								if (!brandsIds.Contains(product.BrandId.Value))
								{
									this.Logger.LogWarning(ReferencedEntityMissing);
									continue;
								}
							}

							if (!productCategoriesIds.Contains(product.CategoryId))
							{
								this.Logger.LogWarning(ReferencedEntityMissing);
								continue;
							}

							Brand brand = await this._context
								.Brands
								.Include(b => b.Products)
								.FirstAsync(b => b.Id == product.BrandId);

							ProductCategory category = await this._context
								.ProductCategories
								.Include(pc => pc.Products)
								.FirstAsync(pc => pc.Id == product.CategoryId);

							brand.Products.Add(product);
							category.Products.Add(product);

							validProducts.Add(product);
						}
					}

					if (validProducts.Count > 0)
					{
						await this._context.Products.AddRangeAsync(validProducts);
						await this._context.SaveChangesAsync();
						this.Logger.LogInformation($"{validProducts.Count} products were added to the DB.");
					}
					else
					{
						this.Logger.LogWarning(NoNewEntityDataToAdd);
					}
				}
			}
			catch (Exception ex)
			{
				this.Logger.LogError(ex.Message);
			}
		}
	}
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineStore.Data.Models;
using OnlineStore.Data.Seeding.Interfaces;
using static OnlineStore.Common.OutputMessages.ErrorMessages;
using System.Text.Json;

namespace OnlineStore.Data.Seeding
{
	public class ProductCategorySeeder : BaseSeeder<ProductCategorySeeder>, IEntitySeeder
	{
		private readonly ApplicationDbContext _context;

		public ProductCategorySeeder(ApplicationDbContext context, ILogger<ProductCategorySeeder> logger) : 
				base(logger)
		{
			this._context = context;
		}

		public override string FilePath =>
					Path.Combine(AppContext.BaseDirectory, "Files", "productCategories.json");

		public async Task SeedEntityData()
		{
			await this.ImportProductCategoriesFromJson();
		}

		private async Task ImportProductCategoriesFromJson()
		{
			string productCategoriesJson = await File.ReadAllTextAsync(this.FilePath);

			List<ProductCategory>? productCategories = JsonSerializer
										.Deserialize<List<ProductCategory>>(productCategoriesJson);

			if (productCategories != null && productCategories.Count > 0)
			{
				var productCategoriesFromDb = await this._context
						.ProductCategories
						.AsNoTracking()
						.Select(pc => new
						{
							pc.Id,
							pc.Name
						})
						.ToListAsync();

				var distinctCategories = productCategories
						.GroupBy(pc => new { pc.Id, pc.Name })
						.Select(g => g.First())
						.ToList();

				var validProductCategories = new List<ProductCategory>();

				foreach (var productCategory in distinctCategories)
				{

					bool isProductCategoryAlreadyExistInDb = productCategoriesFromDb
							.Any(pc => pc.Id == productCategory.Id || pc.Name == productCategory.Name);

					if (isProductCategoryAlreadyExistInDb)
					{
						this.Logger.LogWarning(EntityInstanceAlreadyExists);
						continue;
					}

					validProductCategories.Add(productCategory);
				}

				if (validProductCategories.Count > 0)
				{
					await this._context
						.ProductCategories
						.AddRangeAsync(validProductCategories);

					await this._context.SaveChangesAsync();
					this.Logger.LogInformation($"Successfully imported {validProductCategories.Count} brands.");
				}
				else
				{
					this.Logger.LogWarning(
						$"No new product categories to import from {this.FilePath}");
				}
			}
			else
			{
				this.Logger.LogWarning(
							BuildEntityValidatorWarningMessage(nameof(ProductCategory)));
			}
		}
	}
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineStore.Data.Models;
using OnlineStore.Data.Seeding.Interfaces;
using static OnlineStore.Data.Common.OutputMessages.ErrorMessages;
using System.Text.Json;

namespace OnlineStore.Data.Seeding
{
	public class BrandSeeder : BaseSeeder<BrandSeeder>, IEntitySeeder
	{
		private readonly ApplicationDbContext _context;

		public BrandSeeder(ApplicationDbContext context, ILogger<BrandSeeder> logger) : 
			base(logger)
		{
			this._context = context;
		}

		public override string FilePath =>
			Path.Combine(AppContext.BaseDirectory, "Files", "brands.json");

		public async Task SeedEntityData()
		{
			await this.ImportBrandsFromJson();
		}

		private async Task ImportBrandsFromJson()
		{
			string brandsJson = await File.ReadAllTextAsync(this.FilePath);

			List<Brand>? brands = JsonSerializer
				.Deserialize<List<Brand>>(brandsJson);

			if (brands != null && brands.Count > 0)
			{
				var brandsFromDb = await this._context
					.Brands
					.AsNoTracking()
					.Select(b => new
					{
						b.Id,
						b.Name,
						b.WebsiteUrl,
						b.LogoUrl
					})
					.ToListAsync();

				var distinctBrands = brands
					.GroupBy(b => new { b.Id, b.Name, b.WebsiteUrl, b.LogoUrl})
					.Select(g => g.First())
					.ToList();

				var validBrands = new List<Brand>();
				foreach (var brand in distinctBrands)
				{
					
					bool isBrandAlreadyExistInDb = brandsFromDb
						.Any(b => b.Id == brand.Id || b.Name == brand.Name || 
						          b.WebsiteUrl == brand.WebsiteUrl || b.LogoUrl == brand.LogoUrl);

					if (isBrandAlreadyExistInDb)
					{
						this.Logger.LogWarning(EntityInstanceAlreadyExists);
						continue;
					}

					validBrands.Add(brand);
				}

				if (validBrands.Count > 0)
				{
					await this._context.Brands.AddRangeAsync(validBrands);
					await this._context.SaveChangesAsync();
					this.Logger.LogInformation(
						$"Successfully imported {validBrands.Count} brands.");
				}
				else
				{
					this.Logger.LogWarning(
						$"No new brands to import from {this.FilePath}");
				}
			}
			else
			{
				this.Logger.LogWarning(
							BuildEntityValidatorWarningMessage(nameof(Brand)));
				return;
			}
		}
	}
}

using Microsoft.Extensions.Logging;
using OnlineStore.Data.DTOs;
using OnlineStore.Data.Models;
using OnlineStore.Data.Seeding.Interfaces;
using OnlineStore.Data.Utilities.Interfaces;
using static OnlineStore.Data.Utilities.EntityValidator;
using static OnlineStore.Data.Common.OutputMessages.ErrorMessages;
using Microsoft.EntityFrameworkCore;

namespace OnlineStore.Data.Seeding
{
	public class ProductRatingSeeder : BaseSeeder<ProductRatingSeeder>, IEntitySeeder, IXmlSeeder
	{
		private readonly ApplicationDbContext _context;
		private readonly IXmlHelper _xmlHelper;

		public ProductRatingSeeder(ILogger<ProductRatingSeeder> logger, ApplicationDbContext context, IXmlHelper xmlHelper) : 
					base(logger)
		{

			this._context = context ?? throw new ArgumentNullException(nameof(context));
			this._xmlHelper = xmlHelper ?? throw new ArgumentNullException(nameof(xmlHelper));
		}

		public override string FilePath =>
					   Path.Combine(AppContext.BaseDirectory, "Files", "productsRatings.xml");

		public string RootName => "ProductRatings";

		public IXmlHelper XmlHelper => this._xmlHelper;

		public async Task SeedEntityData()
		{
			await this.ImportProductsRatingsFromXml();
		}

		private async Task ImportProductsRatingsFromXml()
		{

			string xmlString = await File.ReadAllTextAsync(this.FilePath);

			try
			{
				ImportProductRatingDTO[]? productRatingsDTOs = this.XmlHelper
							.Deserialize<ImportProductRatingDTO[]>(xmlString, this.RootName);

				if (productRatingsDTOs != null && productRatingsDTOs.Length > 0)
				{
					ICollection<ProductRating> validProductsRatings = new List<ProductRating>();

					HashSet<int> validProductsIds = (await this._context
						.Products
						.AsNoTracking()
						.Select(p => p.Id)
						.ToListAsync()).ToHashSet();

					HashSet<string> validUsersIds = (await this._context
						.Users
						.AsNoTracking()
						.Select(u => u.Id)
						.ToListAsync()).ToHashSet();

					this.Logger.LogInformation($"Found {productRatingsDTOs.Length} ProductRating DTOs to process.");

					foreach (var productRatingDto in productRatingsDTOs)
					{

						if (!IsValid(productRatingDto))
						{
							string logMessage = this.BuildEntityValidatorWarningMessage(nameof(ProductRating));
							this.Logger.LogWarning(logMessage);
							continue;
						}

						bool isProductIdValid = int.TryParse(productRatingDto.ProductId, out int productId);

						bool isRatingValid = int.TryParse(productRatingDto.Rating, out int rating);

						bool isCreatedAtValid = DateTime.TryParse(productRatingDto.CreatedAt, out DateTime createdAt);

						bool isDeletedValid = bool.TryParse(productRatingDto.IsDeleted, out bool isDeleted);

						if (!isProductIdValid || !isRatingValid || !isCreatedAtValid || !isDeletedValid)
						{
							this.Logger.LogWarning(EntityDataParseError);
							continue;
						}

						if (!validProductsIds.Contains(productId))
						{
							
							this.Logger.LogWarning(ReferencedEntityMissing);
							continue;
						}

						string? userId = null;
						if (!string.IsNullOrWhiteSpace(productRatingDto.UserId))
						{

							if (!validUsersIds.Contains(productRatingDto.UserId))
							{
								this.Logger.LogWarning(ReferencedEntityMissing);
								continue;
							}
							else
							{
								userId = productRatingDto.UserId;
							}
						}

						ProductRating? alreadyMadeRatings = null;

						if (userId != null)
						{

							alreadyMadeRatings = await this._context
								.ProductsRatings
								.AsNoTracking()
								.FirstOrDefaultAsync(pr => pr.ProductId == productId && pr.UserId == userId);

							if (alreadyMadeRatings != null)
							{
								
								this.Logger.LogWarning(EntityInstanceAlreadyExists);
								continue;
							}
						}

						var productRating = new ProductRating()
						{
							ProductId = productId,
							UserId = userId,
							Rating = rating,
							CreatedAt = createdAt,
							IsDeleted = isDeleted
						};

						ApplicationUser user = await this._context
							.Users
							.Include(u => u.ProductRatings)
							.FirstAsync(u => u.Id == userId);

						Product product = await this._context
							.Products
							.Include(p => p.ProductRatings)
							.FirstAsync(p => p.Id == productId);

						user.ProductRatings.Add(productRating);
						product.ProductRatings.Add(productRating);

						validProductsRatings.Add(productRating);
					}

					if (validProductsRatings.Count > 0)
					{
						await this._context.ProductsRatings.AddRangeAsync(validProductsRatings);
						await this._context.SaveChangesAsync();
						this.Logger.LogInformation($"Successfully imported {validProductsRatings.Count} product ratings into the database!");
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
				return;
			}
		}
	}
}

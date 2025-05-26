using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineStore.Data.DTOs;
using OnlineStore.Data.Models;
using OnlineStore.Data.Seeding.Interfaces;
using OnlineStore.Data.Utilities.Interfaces;
using static OnlineStore.Data.Common.OutputMessages.ErrorMessages;
using static OnlineStore.Data.Utilities.EntityValidator;

namespace OnlineStore.Data.Seeding
{
	public class ArticleCategorySeeder : BaseSeeder<ArticleCategorySeeder>, IEntitySeeder, IXmlSeeder
	{
		private readonly IXmlHelper _xmlHelper;
		private readonly ApplicationDbContext _context;

		public ArticleCategorySeeder(ILogger<ArticleCategorySeeder> logger, ApplicationDbContext context, IXmlHelper xmlHelper) : 
					base(logger)
		{

			this._context = context;
			this._xmlHelper = xmlHelper;
		}

		public override string FilePath =>
					Path.Combine(AppContext.BaseDirectory, "Files", "articlesCategories.xml");

		public string RootName => "ArticleCategories";

		public IXmlHelper XmlHelper => this._xmlHelper;

		public async Task SeedEntityData()
		{
			await this.ImportArticleCategoriesFromXml();
		}

		private async Task ImportArticleCategoriesFromXml()
		{
			string articleCategoriesXml = await File.ReadAllTextAsync(this.FilePath);

			try
			{
				ArticleCategoryDTO[]? articleCategoryDTOs = this.XmlHelper
									.Deserialize<ArticleCategoryDTO[]>(articleCategoriesXml, this.RootName);

				if (articleCategoryDTOs != null && articleCategoryDTOs.Length > 0)
				{
					ICollection<ArticleCategory> validArticleCategories = new List<ArticleCategory>();
					HashSet<string> existingArticleCategoriesNames = (await this._context
							.ArticleCategories
							.AsNoTracking()
							.Select(ac => ac.Name)
							.ToListAsync()).ToHashSet();

					this.Logger.LogInformation($"Found {articleCategoryDTOs.Length} ArticlesCategories DTOs to process.");

					foreach (var articleCategoryDto in articleCategoryDTOs)
					{

						if (!IsValid(articleCategoryDto))
						{

							this.Logger.LogWarning(BuildEntityValidatorWarningMessage(nameof(ArticleCategory)));
							continue;
						}

						bool isDeletedValid = bool.TryParse(articleCategoryDto.IsDeleted, out bool isDeleted);

						if (!isDeletedValid)
						{
							this.Logger.LogWarning(EntityDataParseError);
							continue;
						}

						if (existingArticleCategoriesNames.Contains(articleCategoryDto.Name))
						{
							this.Logger.LogWarning(EntityInstanceAlreadyExists);
							continue;
						}

						ArticleCategory articleCategory = new ArticleCategory()
						{
							Name = articleCategoryDto.Name,
							Description = articleCategoryDto.Description,
							IsDeleted = isDeleted
						};

						validArticleCategories.Add(articleCategory);
					}

					if (validArticleCategories.Count > 0)
					{
						await this._context
								.ArticleCategories
								.AddRangeAsync(validArticleCategories);
						await this._context.SaveChangesAsync();
						this.Logger.LogInformation($"{validArticleCategories.Count} article categories were added to the database.");
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

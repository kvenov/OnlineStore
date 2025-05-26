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
	public class ArticleSeeder : BaseSeeder<ArticleSeeder>, IEntitySeeder, IXmlSeeder
	{
		private readonly ApplicationDbContext _context;
		private readonly IXmlHelper _xmlHelper;

		public ArticleSeeder(ILogger<ArticleSeeder> logger, ApplicationDbContext context, IXmlHelper xmlHelper) :
					base(logger)
		{
			this._context = context;
			this._xmlHelper = xmlHelper;
		}

		public override string FilePath =>
				Path.Combine(AppContext.BaseDirectory, "Files", "articles.xml");

		public string RootName => "Articles";

		public IXmlHelper XmlHelper => this._xmlHelper;

		public async Task SeedEntityData()
		{
			await this.ImportArticlesFromXml();
		}

		private async Task ImportArticlesFromXml()
		{
			string xmlString = await File.ReadAllTextAsync(this.FilePath);

			try
			{

				ImportArticlesDTO[]? articlesDTOs = this.XmlHelper
									.Deserialize<ImportArticlesDTO[]>(xmlString, this.RootName);

				if (articlesDTOs != null && articlesDTOs.Length > 0)
				{
					ICollection<Article> validArticles = new List<Article>();

					HashSet<string> validUsersIds = (await this._context
							.Users
							.AsNoTracking()
							.Select(u => u.Id)
							.ToListAsync()).ToHashSet();

					HashSet<int> validArticleCategoriesIds = (await this._context
							.ArticleCategories
							.AsNoTracking()
							.Select(ac => ac.Id)
							.ToListAsync()).ToHashSet();

					this.Logger.LogInformation($"Found {articlesDTOs.Length} Articles DTOs to process.");

					foreach (var articleDto in articlesDTOs)
					{

						if (!IsValid(articleDto))
						{
							string logMessage = this.BuildEntityValidatorWarningMessage(nameof(Article));
							this.Logger.LogWarning(logMessage);
							continue;
						}

						bool isPublishedDateValid = DateTime.TryParse(articleDto.PublishedDate, out DateTime publishedDate);

						bool isPublishedValid = bool.TryParse(articleDto.IsPublished, out bool isPublished);

						bool isCategoryIdValid = int.TryParse(articleDto.CategoryId, out int categoryId);

						bool isDeletedValid = bool.TryParse(articleDto.IsDeleted, out bool isDeleted);

						if (!isPublishedDateValid || !isPublishedValid || !isCategoryIdValid || !isDeletedValid)
						{
							this.Logger.LogWarning(EntityDataParseError);
							continue;
						}

						if (!string.IsNullOrWhiteSpace(articleDto.AuthorId))
						{

							if (!validUsersIds.Contains(articleDto.AuthorId))
							{
								this.Logger.LogWarning(ReferencedEntityMissing);
								continue;
							}
						}

						if (!validArticleCategoriesIds.Contains(categoryId))
						{
							this.Logger.LogWarning(ReferencedEntityMissing);
							continue;
						}

						Article article = new Article()
						{
							Title = articleDto.Title,
							Content = articleDto.Content,
							PublishedDate = publishedDate,
							ImageUrl = articleDto.ImageUrl,
							IsPublished = isPublished,
							AuthorId = articleDto.AuthorId,
							CategoryId = categoryId,
							IsDeleted = isDeleted
						};

						ArticleCategory? articleCategory = await this._context
								.ArticleCategories
								.FirstOrDefaultAsync(ac => ac.Id == categoryId);

						if (articleCategory != null)
						{
							articleCategory.Articles.Add(article);
						}

						validArticles.Add(article);
					}

					if (validArticles.Count > 0)
					{
						await this._context
								.Articles
								.AddRangeAsync(validArticles);
						await this._context.SaveChangesAsync();
						this.Logger.LogInformation($"{validArticles.Count} articles were added to the database.");
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

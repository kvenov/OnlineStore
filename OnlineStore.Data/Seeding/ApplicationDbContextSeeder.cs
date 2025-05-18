using OnlineStore.Data.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using OnlineStore.Data.Utilities.Interfaces;
using OnlineStore.Data.Seeding.Interfaces;

namespace OnlineStore.Data.Seeding
{
    public class ApplicationDbContextSeeder : IDbSeeder
    {
		private readonly ApplicationDbContext _context;

		private readonly UserManager<ApplicationUser> userManager;
		private readonly RoleManager<IdentityRole> roleManager;

		private readonly IXmlHelper _xmlHelper;

		private readonly ICollection<IEntitySeeder> entitySeeders;

		public ApplicationDbContextSeeder(ApplicationDbContext context, UserManager<ApplicationUser> userManager, 
			RoleManager<IdentityRole> roleManager, ILogger<IdentitySeeder> identityLogger, 
			ILogger<ProductCategorySeeder> productCategoryLogger, ILogger<BrandSeeder> brandLogger, ILogger<ProductSeeder> productLogger,
			ILogger<ArticleCategorySeeder> articleCategoryLogger, ILogger<ArticleSeeder> articleLogger, ILogger<ProductRatingSeeder> productRatingLogger, 
			ILogger<PaymentMethodSeeder> paymentMethodLogger, IXmlHelper xmlHelper)
		{
			this._context = context;

			this.userManager = userManager;
			this.roleManager = roleManager;

			this._xmlHelper = xmlHelper;

			this.entitySeeders = new List<IEntitySeeder>();
			this.InitializeDbSeeders(identityLogger, productCategoryLogger, brandLogger, productLogger, 
									articleCategoryLogger, articleLogger, productRatingLogger, paymentMethodLogger);
		}

		public async Task SeedData()
		{

			foreach (IEntitySeeder entitySeeder in this.entitySeeders)
			{
				await entitySeeder.SeedEntityData();
			}
		}

		private void InitializeDbSeeders(ILogger<IdentitySeeder> identityLogger, ILogger<ProductCategorySeeder> productCategoryLogger,
										ILogger<BrandSeeder> brandLogger, ILogger<ProductSeeder> productLogger, ILogger<ArticleCategorySeeder> articleCategoryLogger,
											ILogger<ArticleSeeder> articleLogger, ILogger<ProductRatingSeeder> productRatingLogger, 
											ILogger<PaymentMethodSeeder> paymentMethodLogger)
		{

			this.entitySeeders.Add(new PaymentMethodSeeder(paymentMethodLogger, this._context));
			this.entitySeeders.Add(new ProductRatingSeeder(productRatingLogger, this._context, this._xmlHelper));
			this.entitySeeders.Add(new ArticleSeeder(articleLogger, this._context, this._xmlHelper));
			this.entitySeeders.Add(new ArticleCategorySeeder(articleCategoryLogger, this._context, this._xmlHelper));
			this.entitySeeders.Add(new ProductSeeder(this._context, productLogger));
			this.entitySeeders.Add(new BrandSeeder(this._context, brandLogger));
			this.entitySeeders.Add(new ProductCategorySeeder(this._context, productCategoryLogger));
			this.entitySeeders.Add(new IdentitySeeder(this._context, this.userManager, this.roleManager, identityLogger));
		}
	}
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;
using static OnlineStore.Common.Constants.EntityConstants.ArticleCategory;

namespace OnlineStore.Data.Configurations
{
	public class ArticleCategoryConfiguration : IEntityTypeConfiguration<ArticleCategory>
	{
		public void Configure(EntityTypeBuilder<ArticleCategory> entity)
		{

			entity
				.HasKey(ac => ac.Id);

			entity
				.Property(ac => ac.Name)
				.IsRequired(true)
				.HasMaxLength(ArticleCategoryNameMaxLength);

			entity
				.Property(ac => ac.Description)
				.IsRequired(false)
				.HasMaxLength(ArticleCategoryDescriptionMaxLength);

			entity
				.HasIndex(ac => ac.Name);
		}
	}
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;
using static OnlineStore.Data.Common.Constants.EntityConstants.Article;

namespace OnlineStore.Data.Configurations
{
	public class ArticleConfiguration : IEntityTypeConfiguration<Article>
	{
		public void Configure(EntityTypeBuilder<Article> entity)
		{

			entity
				.HasKey(a => a.Id);

			entity
				.Property(p => p.Title)
				.IsRequired(true)
				.HasMaxLength(ArticleTitleMaxLength);

			entity
				.Property(p => p.Content)
				.IsRequired(true)
				.IsUnicode(true)
				.HasColumnType("nvarchar(max)");

			entity
				.Property(p => p.PublishedDate)
				.IsRequired(true);

			entity
				.Property(p => p.ImageUrl)
				.IsRequired(false)
				.IsUnicode(true)
				.HasMaxLength(ArticleImageUrlMaxLenth);

			entity
				.Property(p => p.IsPublished)
				.IsRequired(true)
				.HasDefaultValue(false);

			entity
				.Property(p => p.AuthorId)
				.IsRequired(false);

			entity
				.Property(p => p.CategoryId)
				.IsRequired(true);

			entity
				.HasOne(a => a.Author)
				.WithMany(au => au.Articles)
				.HasForeignKey(a => a.AuthorId)
				.OnDelete(DeleteBehavior.SetNull);

			entity
				.HasOne(a => a.Category)
				.WithMany(c => c.Articles)
				.HasForeignKey(a => a.CategoryId)
				.OnDelete(DeleteBehavior.Restrict);

			entity
				.HasIndex(p => p.Title);

			entity
				.HasIndex(p => p.PublishedDate);

			entity
				.HasIndex(p => p.IsPublished);

			entity
				.HasQueryFilter(a => (!a.IsDeleted) &&
									 (a.Author == null || !a.Author.IsDeleted) &&
									 (!a.Category.IsDeleted));

		}
	}
}

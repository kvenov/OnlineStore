using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;
using static OnlineStore.Data.Common.Constants.EntityConstants.ProductReview;

namespace OnlineStore.Data.Configurations
{
	public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
	{
		public void Configure(EntityTypeBuilder<ProductReview> entity)
		{
			entity
				.HasKey(pr => pr.Id);

			entity
				.Property(p => p.Content)
				.IsUnicode(true)
				.HasMaxLength(ProductReviewContentMaxLength)
				.IsRequired();

			entity
				.Property(p => p.UpdatedAt)
				.HasColumnType("DATETIME2")
				.IsRequired(false);

			entity
				.Property(p => p.CreatedOn)
				.HasColumnType("DATETIME2")
				.HasDefaultValueSql("GETDATE()")
				.IsRequired(true);

			entity
				.Property(pr => pr.IsDeleted)
				.HasDefaultValue(false)
				.IsRequired(true);

			entity
				.HasOne(pr => pr.Product)
				.WithMany(p => p.ProductReviews)
				.HasForeignKey(pr => pr.ProductId)
				.OnDelete(DeleteBehavior.Restrict);

			entity
				.HasOne(pr => pr.User)
				.WithMany(u => u.ProductReviews)
				.HasForeignKey(pr => pr.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			entity
				.HasIndex(pr => new { pr.UserId, pr.ProductId })
				.IsUnique(true);

			entity
				.HasQueryFilter(wi => wi.Product.IsDeleted == false);
		}
	}
}

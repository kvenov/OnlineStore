using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;
using static OnlineStore.Data.Common.Constants.EntityConstants.ProductCategory;

namespace OnlineStore.Data.Configurations
{
	public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
	{
		public void Configure(EntityTypeBuilder<ProductCategory> entity)
		{
			
			entity
				.HasKey(pc => pc.Id);

			entity
				.Property(pc => pc.Name)
				.IsRequired()
				.HasMaxLength(ProductCategoryNameMaxLength);

			entity
				.Property(pc => pc.Description)
				.IsRequired(false)
				.HasMaxLength(ProductCategoryDescriptionMaxLength);

			entity
				.Property(pc => pc.ParentCategoryId)
				.IsRequired(false);

			entity
				.HasOne(pc => pc.ParentCategory)
				.WithMany(pc => pc.Subcategories)
				.HasForeignKey(pc => pc.ParentCategoryId)
				.OnDelete(DeleteBehavior.Restrict);

			entity
				.HasIndex(pc => pc.Name);
		}
	}
}

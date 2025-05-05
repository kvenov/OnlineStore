using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;
using static OnlineStore.Common.EntityConstants.ProductCategory;

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
				.HasIndex(pc => pc.Name);
		}
	}
}

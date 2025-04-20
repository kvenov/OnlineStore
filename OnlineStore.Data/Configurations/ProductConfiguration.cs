using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;
using static OnlineStore.Common.EntityConstants.Product;

namespace OnlineStore.Data.Configurations
{
	public class ProductConfiguration : IEntityTypeConfiguration<Product>
	{
		public void Configure(EntityTypeBuilder<Product> entity)
		{

			entity
				.HasKey(p => p.Id);

			entity
				.Property(p => p.Name)
				.IsRequired()
				.HasMaxLength(ProductNameMaxLength);

			entity
				.Property(p => p.Description)
				.IsRequired()
				.HasMaxLength(ProductDescriptionMaxLength);

			entity
				.Property(p => p.Price)
				.HasColumnType(ProductPriceType);

			entity
				.Property(p => p.DiscountPrice)
				.HasColumnType(ProductDiscountPriceType);

			entity
				.Property(p => p.StockQuantity)
				.IsRequired();

			entity
				.Property(p => p.IsActive)
				.IsRequired();

			entity
				.Property(p => p.CreatedAt)
				.IsRequired();

			entity
				.Property(p => p.ImageUrl)
				.IsRequired()
				.HasMaxLength(ProductImageUrlMaxLength);

			entity
				.Property(p => p.CategoryId)
				.IsRequired(true);

			entity
				.Property(p => p.BrandId)
				.IsRequired(false);

			entity
				.HasOne(p => p.Category)
				.WithMany(c => c.Products)
				.HasForeignKey(p => p.CategoryId)
				.OnDelete(DeleteBehavior.Restrict);

			entity
				.HasOne(p => p.Brand)
				.WithMany(b => b.Products)
				.HasForeignKey(p => p.BrandId)
				.OnDelete(DeleteBehavior.SetNull);
		}
	}
}

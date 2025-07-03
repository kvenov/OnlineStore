using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;
using static OnlineStore.Data.Common.Constants.EntityConstants.ProductPromotion;

namespace OnlineStore.Data.Configurations
{
	public class ProductPromotionConfiguration : IEntityTypeConfiguration<ProductPromotion>
	{
		public void Configure(EntityTypeBuilder<ProductPromotion> entity)
		{

			entity
				.HasKey(p => p.Id);

			entity
				.Property(p => p.ProductId)
				.IsRequired();

			entity
				.Property(p => p.PromotionPrice)
				.HasColumnType(PromotionPriceType)
				.IsRequired();

			entity
				.Property(p => p.Label)
				.HasMaxLength(LabelMaxLength)
				.IsRequired(true);

			entity
				.Property(p => p.StartDate)
				.HasDefaultValue(DateTime.UtcNow)
				.IsRequired();

			entity
				.Property(p => p.ExpDate)
				.IsRequired();

			entity
				.Property(p => p.IsDeleted)
				.HasDefaultValue(IsDeletedDefaultValue)
				.IsRequired();

			entity
				.HasOne(pp => pp.Product)
				.WithMany(p => p.Promotions)
				.HasForeignKey(p => p.ProductId)
				.OnDelete(DeleteBehavior.Restrict);

			entity
				.HasQueryFilter(p => p.Product.IsDeleted == false);
		}
	}
}

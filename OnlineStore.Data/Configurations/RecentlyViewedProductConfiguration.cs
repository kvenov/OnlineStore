using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;

namespace OnlineStore.Data.Configurations
{
	public class RecentlyViewedProductConfiguration : IEntityTypeConfiguration<RecentlyViewedProduct>
	{
		public void Configure(EntityTypeBuilder<RecentlyViewedProduct> entity)
		{

			entity
				.HasKey(p => p.Id);

			entity
				.Property(p => p.ProductId)
				.IsRequired();

			entity
				.Property(p => p.UserId)
				.IsRequired();

			entity
				.Property(p => p.ViewedAt)
				.IsRequired();

			entity
				.HasOne(p => p.Product)
				.WithMany()
				.HasForeignKey(p => p.ProductId)
				.OnDelete(DeleteBehavior.Cascade);

			entity
				.HasOne(p => p.User)
				.WithMany(u => u.RecentlyViewedProducts)
				.HasForeignKey(p => p.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			entity
				.HasQueryFilter(wi => wi.Product.IsDeleted == false &&
									  wi.User.IsDeleted == false);
		}
	}
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;

namespace OnlineStore.Data.Configurations
{
	public class WishlistItemConfiguration : IEntityTypeConfiguration<WishlistItem>
	{
		public void Configure(EntityTypeBuilder<WishlistItem> entity)
		{

			entity
				.HasKey(p => p.Id);

			entity
				.Property(p => p.AddedAt)
				.IsRequired(true);

			entity 
				.Property(p => p.Notes)
				.IsRequired(false);

			entity
				.Property(p => p.Quantity)
				.IsRequired(false);

			entity
				.Property(p => p.WishlistId)
				.IsRequired(true);

			entity
				.Property(p => p.ProductId)
				.IsRequired(true);

			entity
				.HasOne(wi => wi.Wishlist)
				.WithMany(w => w.WishlistItems)
				.HasForeignKey(wi => wi.WishlistId)
				.OnDelete(DeleteBehavior.Cascade);

			entity
				.HasOne(wi => wi.Product)
				.WithMany(p => p.WishlistItems)
				.HasForeignKey(wi => wi.ProductId)
				.OnDelete(DeleteBehavior.Restrict);

			entity
				.HasQueryFilter(wi => wi.Product.IsDeleted == false &&
									  wi.Wishlist.IsDeleted == false);
		}
	}
}

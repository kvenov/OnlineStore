using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;

namespace OnlineStore.Data.Configurations
{
	public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
	{
		public void Configure(EntityTypeBuilder<Wishlist> entity)
		{

			entity
				.HasKey(p => p.Id);

			entity
				.Property(p => p.UserId)
				.IsRequired(true);

			entity
				.HasOne(w => w.User)
				.WithOne(u => u.Wishlist)
				.HasForeignKey<Wishlist>(w => w.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			entity
				.HasIndex(w => w.UserId)
				.IsUnique();
		}
	}
}

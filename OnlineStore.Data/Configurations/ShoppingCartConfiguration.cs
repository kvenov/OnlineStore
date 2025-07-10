using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;

namespace OnlineStore.Data.Configurations
{
	public class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
	{
		public void Configure(EntityTypeBuilder<ShoppingCart> entity)
		{

			entity
				.HasKey(sc => sc.Id);

			entity
				.Property(sc => sc.CreatedAt)
				.IsRequired()
				.HasDefaultValueSql("GETUTCDATE()");

			entity
				.Property(sc => sc.UserId)
				.IsRequired(false);

			entity
				.Property(sc => sc.GuestId)
				.IsRequired(false);

			entity
				.HasOne(sc => sc.User)
				.WithOne(u => u.ShoppingCart)
				.HasForeignKey<ShoppingCart>(sc => sc.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			entity
				.HasIndex(sc => sc.UserId)
				.IsUnique();

			entity
				.HasIndex(sc => sc.CreatedAt);

			entity.HasQueryFilter(sc =>
				(sc.User != null && !sc.User.IsDeleted) ||
				(sc.User == null && sc.GuestId != null)
			);

		}
	}
}

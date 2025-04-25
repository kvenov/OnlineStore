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
				.IsRequired();

			entity
				.HasOne(sc => sc.User)
				.WithOne(u => u.ShoppingCart)
				.HasForeignKey<ShoppingCart>(sc => sc.UserId)
				.OnDelete(DeleteBehavior.Cascade);

		}
	}
}

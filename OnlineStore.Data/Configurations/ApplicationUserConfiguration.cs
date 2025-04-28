using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static OnlineStore.Common.EntityConstants.ApplicationUser;
using OnlineStore.Data.Models;

namespace OnlineStore.Data.Configurations
{
	public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
	{
		public void Configure(EntityTypeBuilder<ApplicationUser> entity)
		{

			entity
				.Property(p => p.FullName)
				.IsRequired(false)
				.IsUnicode(true)
				.HasMaxLength(UserFullNameMaxLength)
				.HasColumnType("nvarchar");

			entity
				.Property(p => p.CreatedDate)
				.IsRequired(true)
				.HasDefaultValueSql("GETDATE()");

			entity
				.Property(p => p.DefaultAddress)
				.IsRequired(false)
				.IsUnicode(true)
				.HasMaxLength(UserDefaultAddressMaxLength)
				.HasColumnType("nvarchar");

			entity
				.Property(p => p.LoyaltyPoints)
				.IsRequired(false);

			entity
				.HasOne(au => au.ShoppingCart)
				.WithOne(sc => sc.User)
				.HasForeignKey<ApplicationUser>(au => au.ShoppingCartId)
				.OnDelete(DeleteBehavior.Cascade);

			entity
				.HasOne(au => au.Wishlist)
				.WithOne(w => w.User)
				.HasForeignKey<ApplicationUser>(au => au.WishlistId)
				.OnDelete(DeleteBehavior.Cascade);

		}
	}
}

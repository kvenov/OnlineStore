using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static OnlineStore.Data.Common.Constants.EntityConstants.ApplicationUser;
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
				.HasColumnType("NVARCHAR");

			entity
				.Property(p => p.CreatedDate)
				.IsRequired(true)
				.HasDefaultValueSql("GETDATE()");

			entity
				.Property(p => p.DefaultPaymentMethodId)
				.IsRequired(false);

			entity
				.Property(p => p.DefaultPaymentDetailsId)
				.IsRequired(false);

			entity
				.Property(p => p.DefaultShippingAddressId)
				.IsRequired(false);

			entity
				.Property(p => p.DefaultBillingAddressId)
				.IsRequired(false);

			entity
				.HasOne(au => au.ShoppingCart)
				.WithOne(sc => sc.User)
				.HasForeignKey<ShoppingCart>(sc => sc.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			entity
				.HasOne(au => au.Wishlist)
				.WithOne(w => w.User)
				.HasForeignKey<Wishlist>(w => w.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			entity
				.HasOne(au => au.DefaultPaymentMethod)
				.WithMany(pm => pm.MethodUsers)
				.HasForeignKey(au => au.DefaultPaymentMethodId)
				.OnDelete(DeleteBehavior.Restrict);

			entity
				.HasOne(au => au.DefaultPaymentDetails)
				.WithMany(pd => pd.DetailsUsers)
				.HasForeignKey(au => au.DefaultPaymentDetailsId)
				.OnDelete(DeleteBehavior.Restrict);

			entity
				.HasOne(au => au.DefaultShippingAddress)
				.WithMany(a => a.ShippingAddressUsers)
				.HasForeignKey(au => au.DefaultShippingAddressId)
				.OnDelete(DeleteBehavior.Restrict);

			entity
				.HasOne(au => au.DefaultBillingAddress)
				.WithMany(a => a.BillingAddressUsers)
				.HasForeignKey(au => au.DefaultBillingAddressId)
				.OnDelete(DeleteBehavior.Restrict);

			entity
				.HasQueryFilter(au => !au.IsDeleted);
		}
	}
}

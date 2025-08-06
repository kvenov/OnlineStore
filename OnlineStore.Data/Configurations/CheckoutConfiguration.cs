using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;
using static OnlineStore.Data.Common.Constants.EntityConstants.Checkout;

namespace OnlineStore.Data.Configurations
{
	public class CheckoutConfiguration : IEntityTypeConfiguration<Checkout>
	{
		public void Configure(EntityTypeBuilder<Checkout> entity)
		{

			entity
				.HasKey(p => p.Id);

			entity
				.Property(p => p.UserId)
				.IsRequired(false);

			entity
				.Property(p => p.GuestId)
				.IsRequired(false);

			entity
				.Property(p => p.GuestEmail)
				.IsRequired(false);

			entity
				.Property(p => p.GuestName)
				.IsRequired(false);

			entity
				.Property(p => p.ShoppingCartId)
				.IsRequired(true);

			entity
				.Property(p => p.SubTotal)
				.HasColumnType(CheckoutSubTotalType)
				.IsRequired(true);

			entity
				.Property(p => p.PaymentMethodId)
				.IsRequired(true);

			entity
				.Property(p => p.PaymentDetailsId)
				.IsRequired(false);

			entity
				.Property(p => p.StartedAt)
				.IsRequired(true);

			entity
				.Property(p => p.CompletedAt)
				.IsRequired(false);

			entity
				.Property(p => p.ShippingAddressId)
				.IsRequired(false);

			entity
				.Property(p => p.BillingAddressId)
				.IsRequired(false);

			entity
				.Property(p => p.ShippingOption)
				.IsRequired(true);

			entity
				.Property(p => p.EstimatedDeliveryStart)
				.IsRequired(true);

			entity 
				.Property(p => p.EstimatedDeliveryEnd)
				.IsRequired(true);

			entity
				.Property(p => p.ShippingPrice)
				.HasColumnType(CheckoutShippingPriceType)
				.IsRequired(true);

			entity
				.Property(p => p.IsCompleted)
				.HasDefaultValue(IsCompletedDefaultValue);

			entity
				.Property(o => o.TotalPrice)
				.HasComputedColumnSql("[ShippingPrice] + [SubTotal]", stored: true)
				.HasColumnType(CheckoutTotalPriceType)
				.IsRequired(true);


			entity
				.HasOne(p => p.User)
				.WithMany(u => u.Checkouts)
				.HasForeignKey(p => p.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			entity
				.HasOne(p => p.ShoppingCart)
				.WithMany(s => s.Checkouts)
				.HasForeignKey(p => p.ShoppingCartId)
				.OnDelete(DeleteBehavior.Restrict);

			entity
				.HasOne(p => p.PaymentMethod)
				.WithMany(pm => pm.Checkouts)
				.HasForeignKey(p => p.PaymentMethodId)
				.OnDelete(DeleteBehavior.Restrict);

			entity
				.HasOne(p => p.PaymentDetails)
				.WithOne(pd => pd.Checkout)
				.HasForeignKey<Checkout>(p => p.PaymentDetailsId)
				.OnDelete(DeleteBehavior.Restrict);

			entity
				.HasOne(p => p.ShippingAddress)
				.WithMany(a => a.ShippingAddressCheckouts)
				.HasForeignKey(p => p.ShippingAddressId)
				.OnDelete(DeleteBehavior.Restrict);

			entity
				.HasOne(p => p.BillingAddress)
				.WithMany(a => a.BillingAddressCheckouts)
				.HasForeignKey(p => p.BillingAddressId)
				.OnDelete(DeleteBehavior.Restrict);

			entity
				.HasOne(p => p.Order)
				.WithOne(o => o.Checkout)
				.HasForeignKey<Order>(o => o.CheckoutId)
				.OnDelete(DeleteBehavior.Cascade);

			entity
				.HasQueryFilter(c => (!c.IsDeleted) &&
									 ((c.User != null && !c.User.IsDeleted) ||
									  (c.User == null && c.GuestId != null)) &&
									 (!c.PaymentMethod.IsDeleted) &&
									 (c.ShippingAddress == null || !c.ShippingAddress.IsDeleted ||
									  c.BillingAddress == null || !c.BillingAddress.IsDeleted));
		}
	}
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;

using static OnlineStore.Data.Common.Constants.EntityConstants.Order;

namespace OnlineStore.Data.Configurations
{
	public class OrderConfiguration : IEntityTypeConfiguration<Order>
	{
		public void Configure(EntityTypeBuilder<Order> entity)
		{

			entity
				.HasKey(o => o.Id);

			entity
				.Property(o => o.OrderDate)
				.IsRequired()
				.HasDefaultValueSql("CURRENT_TIMESTAMP");

			entity
				.Property(o => o.TotalAmount)
				.IsRequired()
				.HasPrecision(20 ,4)
				.HasColumnType(OrderTotalAmountType);

			entity
				.Property(o => o.ShippingAddressId)
				.IsRequired(true);

			entity
				.Property(o => o.BillingAddressId)
				.IsRequired(true);

			entity
				.Property(o => o.Status)
				.IsRequired();

			entity
				.Property(o => o.UserId)
				.IsRequired(false);

			entity
				.Property(o => o.GuestId)
				.IsRequired(false);

			entity
				.Property(o => o.GuestName)
				.IsRequired(false);

			entity
				.Property(o => o.GuestEmail)
				.IsRequired(false);

			entity
				.Property(o => o.OrderNumber)
				.IsRequired(true);

			entity
				.Property(o => o.PaymentMethodId)
				.IsRequired(true);

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
				.HasColumnType(OrderShippingPriceType)
				.IsRequired(true);

			entity
				.Property(p => p.IsCompleted)
				.HasDefaultValue(IsCompletedDefaultValue);

			entity
				.Property(p => p.IsCancelled)
				.HasDefaultValue(IsCancelledDefaultValue);

			entity
				.Property(p => p.IsDeleted)
				.HasDefaultValue(IsDeletedDefaultValue);

			entity
				.HasOne(o => o.User)
				.WithMany(u => u.Orders)
				.HasForeignKey(o => o.UserId)
				.OnDelete(DeleteBehavior.SetNull);

			entity
				.HasOne(o => o.PaymentMethod)
				.WithMany(pm => pm.Orders)
				.HasForeignKey(o => o.PaymentMethodId)
				.OnDelete(DeleteBehavior.Restrict);

			entity
				.HasOne(o => o.PaymentDetails)
				.WithOne(pd => pd.Order)
				.HasForeignKey<PaymentDetails>(pd => pd.OrderId)
				.OnDelete(DeleteBehavior.Cascade);

			entity
				.HasOne(o => o.ShippingAddress)
				.WithMany(a => a.ShippingAddressOrders)
				.HasForeignKey(o => o.ShippingAddressId)
				.OnDelete(DeleteBehavior.Restrict);

			entity
				.HasOne(o => o.BillingAddress)
				.WithMany(a => a.BillingAddressOrders)
				.HasForeignKey(o => o.BillingAddressId)
				.OnDelete(DeleteBehavior.Restrict);

			entity
				.HasOne(o => o.Checkout)
				.WithOne(c => c.Order)
				.HasForeignKey<Order>(o => o.CheckoutId)
				.OnDelete(DeleteBehavior.Cascade);

			entity
				.HasIndex(o => o.OrderDate);

			entity
				.HasIndex(o => o.UserId);

			entity
				.HasIndex(o => o.GuestId);

			entity
				.HasIndex(o => o.Status);

			entity
				.HasQueryFilter(o =>  !o.IsDeleted &&
									  (o.User == null || !o.User.IsDeleted) &&
									  o.PaymentMethod.IsDeleted == false &&
									  o.Checkout.IsDeleted == false &&
									  o.BillingAddress.IsDeleted == false &&
									  o.ShippingAddress.IsDeleted == false);

		}
	}
}

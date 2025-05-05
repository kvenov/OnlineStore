using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;
using static OnlineStore.Common.EntityConstants.Order;

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
				.Property(o => o.ShippingAddress)
				.IsRequired()
				.HasMaxLength(OrderShippingAddressMaxLength);

			entity
				.Property(o => o.Status)
				.IsRequired();

			entity
				.Property(o => o.UserId)
				.IsRequired(false);

			entity
				.Property(o => o.PaymentMethodId)
				.IsRequired(true);

			entity
				.Property(o => o.PaymentDetailsId)
				.IsRequired(true);

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
				.HasForeignKey<Order>(o => o.PaymentDetailsId)
				.OnDelete(DeleteBehavior.Cascade);

			entity
				.HasOne(o => o.ShippingAddress)
				.WithMany()
				.HasForeignKey(o => o.ShippingAddressId)
				.OnDelete(DeleteBehavior.Restrict);

			entity
				.HasOne(o => o.BillingAddress)
				.WithMany()
				.HasForeignKey(o => o.BillingAddressId)
				.OnDelete(DeleteBehavior.Restrict);

			entity
				.HasIndex(o => o.OrderDate);

			entity
				.HasIndex(o => o.UserId);

			entity
				.HasIndex(o => o.Status);

		}
	}
}

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
				.IsRequired();

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
				.HasOne(o => o.User)
				.WithMany(u => u.Orders)
				.HasForeignKey(o => o.UserId)
				.OnDelete(DeleteBehavior.SetNull);
		}
	}
}

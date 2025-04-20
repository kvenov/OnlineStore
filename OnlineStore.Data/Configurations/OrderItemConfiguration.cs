using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;
using static OnlineStore.Common.EntityConstants.OrderItem;

namespace OnlineStore.Data.Configurations
{
	public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
	{
		public void Configure(EntityTypeBuilder<OrderItem> entity)
		{

			entity
				.HasKey(oi => oi.Id);

			entity
				.Property(p => p.Quantity)
				.IsRequired();

			entity
				.Property(p => p.UnitPrice)
				.HasColumnType(OrderItemUnitPriceType)
				.IsRequired();

			entity
				.Property(p => p.Subtotal)
				.HasComputedColumnSql("[Quantity] * [UnitPrice]")
				.HasColumnType(OrderItemSubtotalPriceType);

			entity
				.HasOne(oi => oi.Order)
				.WithMany(o => o.OrderItems)
				.HasForeignKey(oi => oi.OrderId);

			entity
				.HasOne(oi => oi.Product)
				.WithMany(p => p.OrderItems)
				.HasForeignKey(oi => oi.ProductId);
		}
	}
}

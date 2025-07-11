using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;
using static OnlineStore.Data.Common.Constants.EntityConstants.ShoppingCartItem;

namespace OnlineStore.Data.Configurations
{
	public class ShoppingCartItemConfiguration : IEntityTypeConfiguration<ShoppingCartItem>
	{
		public void Configure(EntityTypeBuilder<ShoppingCartItem> entity)
		{

			entity
				.HasKey(e => e.Id);

			entity
				.Property(p => p.Quantity)
				.IsRequired();

			entity
				.Property(p => p.Price)
				.HasColumnType(ShoppingCartItemPriceType)
				.IsRequired();

			entity
				.Property(p => p.TotalPrice)
				.HasColumnType(ShoppingCartItemTotalPriceType)
				.HasComputedColumnSql("[Price] * [Quantity]")
				.IsRequired();

			entity
				.HasOne(sci => sci.ShoppingCart)
				.WithMany(sc => sc.ShoppingCartItems)
				.HasForeignKey(sci => sci.ShoppingCartId)
				.OnDelete(DeleteBehavior.Cascade);

			entity
				.HasOne(sci => sci.Product)
				.WithMany(p => p.ShoppingCartItems)
				.HasForeignKey(sci => sci.ProductId)
				.OnDelete(DeleteBehavior.Restrict);

			entity
				.HasQueryFilter(sci => sci.Product.IsDeleted == false);
		}
	}
}

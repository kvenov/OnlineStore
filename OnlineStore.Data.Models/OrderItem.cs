using Microsoft.EntityFrameworkCore;

namespace OnlineStore.Data.Models
{

	[Comment("Order items in the store")]
	public class OrderItem
	{

		[Comment("Order item identifier")]
		public int Id { get; set; }

		[Comment("Order item quantity")]
		public int Quantity { get; set; }

		[Comment("Order item current price")]
		public decimal UnitPrice { get; set; }

		[Comment("Order item price")]
		public decimal Subtotal { get; set; }


		public int OrderId { get; set; }
		[Comment("Order from where the order item is contained")]
		public virtual Order Order { get; set; } = null!;

		public int ProductId { get; set; }
		[Comment("Order item product")]
		public virtual Product Product { get; set; } = null!;

		[Comment("Order item ProductSize")]
		public string ProductSize { get; set; } = null!;
	}
}

using Microsoft.EntityFrameworkCore;

namespace OnlineStore.Data.Models
{

	[Comment("Shopping cart items in the store")]
	public class ShoppingCartItem
	{

		[Comment("Shopping cart item identifier")]
		public int Id { get; set; }

		[Comment("Shopping cart item quantity")]
		public int Quantity { get; set; }

		[Comment("Shopping cart item current price")]
		public decimal Price { get; set; }

		[Comment("Shopping cart item total price")]
		public decimal TotalPrice { get; set; }


		public int ShoppingCartId { get; set; }

		[Comment("Shopping cart")]
		public virtual ShoppingCart ShoppingCart { get; set; } = null!;

		public int ProductId { get; set; }

		[Comment("Shopping cart item product")]
		public virtual Product Product { get; set; } = null!;
	}
}
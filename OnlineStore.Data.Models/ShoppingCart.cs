using Microsoft.EntityFrameworkCore;

namespace OnlineStore.Data.Models
{

	[Comment("Shopping carts in the store")]
	public class ShoppingCart
	{
		[Comment("Shopping cart identifier")]
		public int Id { get; set; }

		[Comment("Shopping cart creation date")]
		public DateTime CreatedAt { get; set; }


		public string UserId { get; set; } = null!;

		[Comment("Owner of the cart")]
		public virtual ApplicationUser User { get; set; } = null!;

		[Comment("Shopping cart items")]
		public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = 
					new HashSet<ShoppingCartItem>();

	}
}

using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models.Enums;

namespace OnlineStore.Data.Models
{

	[Comment("Orders in the store")]
	public class Order
	{

		[Comment("Order identifier")]
		public int Id { get; set; }

		[Comment("Order creation date")]
		public DateTime OrderDate { get; set; }

		[Comment("Order total amount")]
		public decimal TotalAmount { get; set; }

		[Comment("Order status")]
		public OrderStatus Status { get; set; }

		[Comment("Order Shipping address")]
		public string ShippingAddress { get; set; } = null!;


		public string? UserId { get; set; }

		[Comment("User who placed the order")]
		public virtual ApplicationUser? User { get; set; }


		[Comment("Items in the order")]
		public virtual ICollection<OrderItem> OrderItems { get; set; } = 
					new HashSet<OrderItem>();
	}
}

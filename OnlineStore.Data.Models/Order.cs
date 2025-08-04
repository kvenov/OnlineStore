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

		public int ShippingAddressId { get; set; }

		[Comment("Order Shipping address")]
		public virtual Address ShippingAddress { get; set; } = null!;

		public int BillingAddressId { get; set; }

		[Comment("Order Billing address")]
		public virtual Address BillingAddress { get; set; } = null!;



		public int PaymentMethodId { get; set; }

		[Comment("Payment method used for the order")]
		public virtual PaymentMethod PaymentMethod { get; set; } = null!;

		[Comment("Payment details used for the order")]
		public virtual PaymentDetails? PaymentDetails { get; set; }

		public string? UserId { get; set; }

		[Comment("User who placed the order")]
		public virtual ApplicationUser? User { get; set; }

		[Comment("Unique guest identifier for order")]
		public string? GuestId { get; set; }

		[Comment("Full name (for guest)")]
		public string? GuestName { get; set; }

		[Comment("Contact email (for guest)")]
		public string? GuestEmail { get; set; }


		[Comment("Order number")]
		public string OrderNumber { get; set; } = null!;

		[Comment("Shipping option for the order")]
		public string ShippingOption { get; set; } = null!;

		[Comment("Estimated delivery start date")]
		public DateTime EstimatedDeliveryStart { get; set; }

		[Comment("Estimated delivery end date")]
		public DateTime EstimatedDeliveryEnd { get; set; }

		[Comment("Shipping price for the order")]
		public decimal ShippingPrice { get; set; }

		public int CheckoutId { get; set; }

		[Comment("The Checkout that is used to create the Order")]
		public virtual Checkout Checkout { get; set; } = null!;


		[Comment("Items in the order")]
		public virtual ICollection<OrderItem> OrderItems { get; set; } = 
					new HashSet<OrderItem>();
	}
}

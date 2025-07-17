using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models.Interfaces;

namespace OnlineStore.Data.Models
{

	[Comment("The checkouts in the store")]
	public class Checkout : ISoftDeletable
	{

		[Comment("Checkout id")]
		public int Id { get; set; }

		public string? UserId { get; set; }

		[Comment("User who started the checkout")]
		public virtual ApplicationUser? User { get; set; }

		[Comment("Unique guest identifier for checkout")]
		public string? GuestId { get; set; }

		[Comment("Contact email (for guest)")]
		public string? GuestEmail { get; set; }

		[Comment("Full name (for guest)")]
		public string? GuestName { get; set; }

		public int ShoppingCartId { get; set; }

		[Comment("Shopping cart used for the checkout")]
		public virtual ShoppingCart ShoppingCart { get; set; } = null!;

		[Comment("Checkout total price")]
		public decimal TotalPrice { get; set; }

		public int PaymentMethodId { get; set; }

		[Comment("Payment method used for the checkout")]
		public virtual PaymentMethod PaymentMethod { get; set; } = null!;

		public int? PaymentDetailsId { get; set; }

		[Comment("Payment details used for the checkout")]
		public virtual PaymentDetails? PaymentDetails { get; set; }

		[Comment("Checkout creation data")]
		public DateTime StartedAt { get; set; }

		[Comment("Checkout date of complition")]
		public DateTime? CompletedAt { get; set; }

		public int? ShippingAddressId { get; set; }

		[Comment("Checkout Shipping address")]
		public virtual Address? ShippingAddress { get; set; }

		public int? BillingAddressId { get; set; }

		[Comment("Checkout Billing address")]
		public virtual Address? BillingAddress { get; set; }

		[Comment("Order that is made of the checkout")]
		public virtual Order? Order { get; set; }

		public bool IsDeleted { get; set; }
	}
}

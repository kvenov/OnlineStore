using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models.Interfaces;

namespace OnlineStore.Data.Models
{

	[Comment("The Addresses of the store")]
	public class Address : ISoftDeletable
	{

		[Comment("The Address identifier")]
		public int Id { get; set; }

		[Comment("The Address Street")]
		public string Street { get; set; } = null!;

		[Comment("The Address City")]
		public string City { get; set; } = null!;

		[Comment("The Address Country")]
		public string Country { get; set; } = null!;

		[Comment("The Address ZipCode")]
		public string ZipCode { get; set; } = null!;

		[Comment("The Address PhoneNumber")]
		public string PhoneNumber { get; set; } = null!;

		[Comment("The Address additional identifier")]
		public bool IsBillingAddress { get; set; }

		[Comment("The Address additional identifier")]
		public bool IsShippingAddress { get; set; }

		public string? UserId { get; set; }

		[Comment("The Address User")]
		public virtual ApplicationUser? User { get; set; }

		[Comment("Unique guest identifier for address")]
		public string? GuestId { get; set; }

		[Comment("The Orders that use this address as Shipping Address")]
		public virtual ICollection<Order> ShippingAddressOrders { get; set; } =
					new HashSet<Order>();

		[Comment("The Orders that use this address as Billing Address")]
		public virtual ICollection<Order> BillingAddressOrders { get; set; } =
					new HashSet<Order>();

		[Comment("The Checkouts that use this address as Shipping Address")]
		public virtual ICollection<Checkout> ShippingAddressCheckouts { get; set; } =
					new HashSet<Checkout>();

		[Comment("The Checkouts that use this address as Billing Address")]
		public virtual ICollection<Checkout> BillingAddressCheckouts { get; set; } =
					new HashSet<Checkout>();

		[Comment("The Users that uses this address as default shipping address")]
		public virtual ICollection<ApplicationUser> ShippingAddressUsers { get; set; } =
					new HashSet<ApplicationUser>();

		[Comment("The Users that uses this address as default billing address")]
		public virtual ICollection<ApplicationUser> BillingAddressUsers { get; set; } =
					new HashSet<ApplicationUser>();

		public bool IsDeleted { get; set; }
	}
}

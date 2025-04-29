using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models.Interfaces;

namespace OnlineStore.Data.Models
{

	[Comment("The Addresses of the store")]
	public class Address : ISoftDeletable
	{

		[Comment("The Address identifier")]
		public int Id { get; set; }

		[Comment("The Address Fullname")]
		public string FullName { get; set; } = null!;

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

		public string UserId { get; set; } = null!;

		[Comment("The Address User")]
		public virtual ApplicationUser User { get; set; } = null!;

		public bool IsDeleted { get; set; }
	}
}

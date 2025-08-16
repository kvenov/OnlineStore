using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models.Enums;

namespace OnlineStore.Data.Models
{

	[Comment("The payment details of the store")]
	public class PaymentDetails
	{

		[Comment("Payment details id")]
		public int Id { get; set; }

		[Comment("Payment details card number")]
		public string CardNumber { get; set; } = null!;

		[Comment("Payment details card brand")]
		public string? CardBrand { get; set; }

		[Comment("Payment details card expiry data month")]
		public int ExpMonth { get; set; }

		[Comment("Payment details card expiry data year")]
		public int ExpYear { get; set; }

		[Comment("Payment details card name")]
		public string NameOnCard { get; set; } = null!;

		[Comment("Payment details payment status")]
		public PaymentStatus Status { get; set; }


		[Comment("The Orders that uses the payment details")]
		public virtual ICollection<Order> Orders { get; set; } =
						new HashSet<Order>();

		[Comment("The Checkouts that uses the payment details")]
		public virtual ICollection<Checkout> Checkouts { get; set; } = 
						new HashSet<Checkout>();


		[Comment("The Users that uses this payment details as default")]
		public virtual ICollection<ApplicationUser> DetailsUsers { get; set; } =
					new HashSet<ApplicationUser>();

	}
}

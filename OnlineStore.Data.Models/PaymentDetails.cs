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
		public int? ExpMonth { get; set; }

		[Comment("Payment details card expiry data year")]
		public int? ExpYear { get; set; }

		[Comment("Payment details card name")]
		public string NameOnCard { get; set; } = null!;

		[Comment("Payment details payment date")]
		public DateTime? PaidAt { get; set; }

		[Comment("Payment details payment status")]
		public PaymentStatus Status { get; set; }


		public int OrderId { get; set; }

		[Comment("Order that uses the payment details")]
		public virtual Order Order { get; set; } = null!;

		[Comment("Checkout that uses the payment details")]
		public virtual Checkout Checkout { get; set; } = null!;

	}
}

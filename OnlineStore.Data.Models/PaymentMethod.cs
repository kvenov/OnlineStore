using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models.Enums;

namespace OnlineStore.Data.Models
{

	[Comment("The payment methods in the store")]
	public class PaymentMethod
	{

		[Comment("Payment method id")]
		public int Id { get; set; }

		[Comment("Payment method name")]
		public string Name { get; set; } = null!;

		[Comment("Payment method code")]
		public PaymentMethodCode? Code { get; set; }

		[Comment("Payment method short admin info")]	
		public bool IsActive { get; set; }


		[Comment("Orders that use the paymnet method")]
		public virtual ICollection<Order> Orders { get; set; } =
					new HashSet<Order>();
	}
}

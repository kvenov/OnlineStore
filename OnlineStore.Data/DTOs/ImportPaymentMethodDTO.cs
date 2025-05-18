using System.ComponentModel.DataAnnotations;
using static OnlineStore.Common.Constants.EntityConstants.PaymentMethod;

namespace OnlineStore.Data.DTOs
{
	public class ImportPaymentMethodDTO
	{

		[Required]
		[MaxLength(PaymentMethodNameMaxLength)]
		public string Name { get; set; } = null!;

		public string? Code { get; set; }

		[Required]
		public bool IsActive { get; set; }

		[Required]
		public bool IsDeleted { get; set; }
	}
}

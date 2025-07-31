using OnlineStore.Web.Validation;
using System.ComponentModel.DataAnnotations;

using static OnlineStore.Common.ApplicationConstants.CreditCardValidationConstants;
using static OnlineStore.Common.ErrorMessages.CreditCardValidationMessages;

namespace OnlineStore.Web.ViewModels.Checkout.Partials
{
	public class CreditCardFormViewModel
	{

		[Required(ErrorMessage = Required)]
		[StringLength(NameOnCardMaxLength, MinimumLength = NameOnCardMinLength, ErrorMessage = NameOnCardLength)]
		[Display(Name = "Name on Card")]
		public string NameOnCard { get; set; } = string.Empty;

		[Required(ErrorMessage = Required)]
		[StringLength(CardNumberMaxLength, MinimumLength = CardNumberMinLength, ErrorMessage = CardNumberLength)]
		[RegularExpression(@"^\d+$", ErrorMessage = CardNumberInvalid)]
		[Display(Name = "Card Number")]
		public string CardNumber { get; set; } = string.Empty;

		[Required(ErrorMessage = Required)]
		[Range(ExpMonthMin, ExpMonthMax, ErrorMessage = ExpirationMonthRange)]
		[Display(Name = "Expiry Month")]
		public int ExpMonth { get; set; }

		[Required(ErrorMessage = Required)]
		[Range(ExpYearMin, ExpYearMax, ErrorMessage = ExpirationYearRange)]
		[CardExpiration(nameof(ExpMonth), ErrorMessage = ExpirationDateInvalid)]
		[Display(Name = "Expiry Year")]
		public int ExpYear { get; set; }

		[Required(ErrorMessage = Required)]
		[StringLength(CvvMaxLength, MinimumLength = CvvMinLength, ErrorMessage = CVVLength)]
		[RegularExpression(@"^\d{3,4}$", ErrorMessage = CVVLength)]
		[Display(Name = "CVV")]
		public string CVV { get; set; } = string.Empty;

		public bool DefaultPaymentDetails { get; set; } = false;
	}
}

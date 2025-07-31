using System.ComponentModel.DataAnnotations;

using static OnlineStore.Common.ApplicationConstants.AddressValidationConstants;
using static OnlineStore.Common.ErrorMessages.AddressValidationMessages;

namespace OnlineStore.Web.ViewModels.Checkout.Partials
{
	public class GuestBillingAddressViewModel
	{
		[Required(ErrorMessage = Required)]
		[StringLength(PhoneMaxLength, MinimumLength = PhoneMinLength, ErrorMessage = PhoneLength)]
		[RegularExpression(PhonePattern, ErrorMessage = PhoneInvalid)]
		[Phone(ErrorMessage = PhoneInvalid)]
		public string BillingPhoneNumber { get; set; } = string.Empty;

		[Required(ErrorMessage = Required)]
		[StringLength(StreetMaxLength, MinimumLength = StreetMinLength, ErrorMessage = StreetLength)]
		public string BillingStreet { get; set; } = string.Empty;

		[Required(ErrorMessage = Required)]
		[StringLength(CityMaxLength, MinimumLength = CityMinLength, ErrorMessage = CityLength)]
		[RegularExpression(CityPattern, ErrorMessage = CityInvalid)]
		public string BillingCity { get; set; } = string.Empty;

		[Required(ErrorMessage = Required)]
		[StringLength(ZipCodeMaxLength, MinimumLength = ZipCodeMinLength, ErrorMessage = ZipCodeLength)]
		[RegularExpression(ZipPattern, ErrorMessage = ZipCodeInvalid)]
		public string BillingZipCode { get; set; } = string.Empty;

		[Required(ErrorMessage = Required)]
		[StringLength(CountryMaxLength, MinimumLength = CountryMinLength, ErrorMessage = CountryLength)]
		[RegularExpression(CountryPattern, ErrorMessage = CountryInvalid)]
		public string BillingCountry { get; set; } = string.Empty;

		public bool DefaultBilling { get; set; } = false;


		public bool IsEmpty()
		{
			return string.IsNullOrWhiteSpace(BillingStreet)
				&& string.IsNullOrWhiteSpace(BillingCity)
				&& string.IsNullOrWhiteSpace(BillingZipCode)
				&& string.IsNullOrWhiteSpace(BillingCountry)
				&& string.IsNullOrWhiteSpace(BillingPhoneNumber);
		}
	}
}

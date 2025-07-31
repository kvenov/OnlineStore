using System.ComponentModel.DataAnnotations;
using static OnlineStore.Common.ApplicationConstants.AddressValidationConstants;
using static OnlineStore.Common.ErrorMessages.AddressValidationMessages;

namespace OnlineStore.Web.ViewModels.Checkout.Partials
{
	public class MemberAddressItemViewModel
	{

		public int? Id { get; set; }

		[Required(ErrorMessage = Required)]
		[StringLength(PhoneMaxLength, MinimumLength = PhoneMinLength, ErrorMessage = PhoneLength)]
		[RegularExpression(PhonePattern, ErrorMessage = PhoneInvalid)]
		[Phone(ErrorMessage = PhoneInvalid)]
		public string PhoneNumber { get; set; } = string.Empty;

		[Required(ErrorMessage = Required)]
		[StringLength(StreetMaxLength, MinimumLength = StreetMinLength, ErrorMessage = StreetLength)]
		public string Street { get; set; } = string.Empty;

		[Required(ErrorMessage = Required)]
		[StringLength(CityMaxLength, MinimumLength = CityMinLength, ErrorMessage = CityLength)]
		[RegularExpression(CityPattern, ErrorMessage = CityInvalid)]
		public string City { get; set; } = string.Empty;

		[Required(ErrorMessage = Required)]
		[StringLength(ZipCodeMaxLength, MinimumLength = ZipCodeMinLength, ErrorMessage = ZipCodeLength)]
		[RegularExpression(ZipPattern, ErrorMessage = ZipCodeInvalid)]
		public string ZipCode { get; set; } = string.Empty;

		[Required(ErrorMessage = Required)]
		[StringLength(CountryMaxLength, MinimumLength = CountryMinLength, ErrorMessage = CountryLength)]
		[RegularExpression(CountryPattern, ErrorMessage = CountryInvalid)]
		public string Country { get; set; } = string.Empty;

		public bool DefaultShipping { get; set; } = false;

		public bool DefaultBilling { get; set; } = false;

		public bool? IsShippingAddress { get; set; }

		public bool IsEmpty()
		{
			return string.IsNullOrWhiteSpace(Street)
				&& string.IsNullOrWhiteSpace(City)
				&& string.IsNullOrWhiteSpace(ZipCode)
				&& string.IsNullOrWhiteSpace(Country)
				&& string.IsNullOrWhiteSpace(PhoneNumber);
		}
	}
}

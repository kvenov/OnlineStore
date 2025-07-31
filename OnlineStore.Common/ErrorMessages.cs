namespace OnlineStore.Common
{
	public class ErrorMessages
	{

		public static class AddressValidationMessages
		{
			public const string Required = "This field is required.";
			public const string FullNameLength = "Full name must be between {2} and {1} characters.";
			public const string PhoneLength = "Phone number must be between {2} and {1} characters.";
			public const string PhoneInvalid = "Invalid phone number format.";
			public const string EmailInvalid = "Email address format is not valid.";
			public const string EmailLength = "Email cannot exceed {1} characters.";
			public const string StreetLength = "Street name must be between {2} and {1} characters.";
			public const string CityLength = "City name must be between {2} and {1} characters.";
			public const string ZipCodeLength = "Zip code must be between {2} and {1} characters.";
			public const string CountryLength = "Country name must be between {2} and {1} characters.";
			public const string CityInvalid = "City name must contain only letters and spaces.";
			public const string ZipCodeInvalid = "Invalid ZIP code format.";
			public const string CountryInvalid = "Country name must contain only letters and spaces.";
		}

		public static class CreditCardValidationMessages
		{
			public const string Required = "This field is required.";
			public const string NameOnCardLength = "Cardholder name must be between {2} and {1} characters.";
			public const string CardNumberLength = "Card number must be between {2} and {1} digits.";
			public const string CardNumberInvalid = "Card number is invalid.";
			public const string ExpirationMonthRange = "Expiry month must be between {1} and {2}.";
			public const string ExpirationYearRange = "Expiry year must be between {1} and {2}.";
			public const string ExpirationDateInvalid = "Card has expired.";
			public const string CVVLength = "CVV must be 3 or 4 digits.";
		}
	}
}

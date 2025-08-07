namespace OnlineStore.Common
{
	public static class ApplicationConstants
	{

		public const string GuestCookieName = "guest_id";

		public const string UserRoleName = "User";
		public const string AdminRoleName = "Admin";

		public const decimal MinPriceForFreeShipping = 300.00m;
		public const decimal StandartShippingPriceForGuests = 20.00m;
		public const decimal StandartShippingPriceForMembers = 0.00m;
		public const decimal ExpressShippingPrice = 30.00m;


		public const string DefaultPaymentMethodName = "Credit Card";
		public const string DefaultPaymentMethodCode = "CreditCard";

		public const string StandartShippingOptionName = "Standard Delivery";
		public const string ExpressShippingOptionName = "Express Delivery";

		public const int StandartShippingOptionDaysMin = 5;
		public const int StandartShippingOptionDaysMax = 7;

		public const int ExpressShippingOptionDaysMin = 2;
		public const int ExpressShippingOptionDaysMax = 3;

		public static class Account
		{
			public const string LoginPath = "/Account/Login";
			public const string RegisterPath = "/Account/Register";
		}

		public static class AddressValidationConstants
		{
			public const int FullNameMaxLength = 100;
			public const int FullNameMinLength = 1;

			public const int EmailMaxLength = 100;

			public const int PhoneMaxLength = 20;
			public const int PhoneMinLength = 7;

			public const int StreetMaxLength = 100;
			public const int StreetMinLength = 5;

			public const int CityMaxLength = 50;
			public const int CityMinLength = 2;

			public const int ZipCodeMaxLength = 10;
			public const int ZipCodeMinLength = 4;

			public const int CountryMaxLength = 50;
			public const int CountryMinLength = 2;

			public const string CityPattern = @"^[A-Za-z\s\-']{2,50}$";
			public const string ZipPattern = @"^\d{4,10}$";
			public const string CountryPattern = @"^[A-Za-z\s]{2,50}$";
			public const string PhonePattern = @"^\+?[0-9\s\-()]{7,20}$";

		}

		public static class CreditCardValidationConstants
		{
			public const int NameOnCardMinLength = 2;
			public const int NameOnCardMaxLength = 100;

			public const int CardNumberMinLength = 13;
			public const int CardNumberMaxLength = 19;

			public const int CvvMinLength = 3;
			public const int CvvMaxLength = 4;

			public const int ExpMonthMin = 1;
			public const int ExpMonthMax = 12;

			public const int ExpYearMin = 25;
			public const int ExpYearMax = 100;

			public const string CardNumberPattern = @"^[\d\*]+$";
			public const string CVVPattern = @"^\d{3,4}$";

		}
	}
}

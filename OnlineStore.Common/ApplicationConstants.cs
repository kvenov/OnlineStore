namespace OnlineStore.Common
{
	public static class ApplicationConstants
	{

		public const string GuestCookieName = "guest_id";


		public const decimal MinPriceForFreeShipping = 300.00m;
		public const decimal StandartShippingPriceForGuests = 20.00m;
		public const decimal StandartShippingPriceForMembers = 0.00m;
		public const decimal ExpressShippingPrice = 30.00m;


		public const string DefaultPaymentMethodName = "Credit Card";
	}
}

namespace OnlineStore.Web.ViewModels.Checkout.Partials
{
	public class PaymentMethodViewModel
	{

		public PaymentOption SelectedPaymentOption { get; set; }

		public CreditCardFormViewModel CreditCardDetails { get; set; } = new();

		public enum PaymentOption
		{
			CreditCard = 0,
			PayPal = 1,
			GooglePay = 4,
			CashOnDelivery = 5
		}
	}
}

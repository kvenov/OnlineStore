using OnlineStore.Data.Models.Enums;

namespace OnlineStore.Web.ViewModels.Checkout.Partials
{
	public class PaymentMethodViewModel
	{

		public PaymentMethodCode SelectedPaymentOption { get; set; }

		public CreditCardFormViewModel CreditCardDetails { get; set; } = new();

	}
}

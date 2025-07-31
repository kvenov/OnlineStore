using OnlineStore.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Web.ViewModels.Checkout.Partials
{
	public class PaymentMethodViewModel
	{

		[Required]
		[AllowedValues(PaymentMethodCode.CreditCard, PaymentMethodCode.PayPal, PaymentMethodCode.GooglePay, PaymentMethodCode.CashOnDelivery)]
		public PaymentMethodCode SelectedPaymentOption { get; set; }

		public CreditCardFormViewModel? CreditCardDetails { get; set; }

		public bool DefaultPaymentMethod { get; set; } = false;

	}
}

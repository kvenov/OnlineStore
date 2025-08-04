using Microsoft.AspNetCore.Mvc.Filters;
using OnlineStore.Data.Models.Enums;
using OnlineStore.Web.ViewModels.Checkout;
using OnlineStore.Web.ViewModels.Checkout.Partials;

namespace OnlineStore.Web.Infrastructure.Filters
{
	public class CleanInvalidPaymentDetailsFilter : IActionFilter
	{
		private const string PaymentDetailsKey = "CreditCardDetails";

		public void OnActionExecuting(ActionExecutingContext context)
		{
			if (context.ActionArguments.TryGetValue("model", out var obj)
					&& obj is CheckoutViewModel model)
			{
				var modelState = context.ModelState;

				if (model.Payment != null)
				{
					PaymentMethodViewModel payment = model.Payment;
					PaymentMethodCode paymentMethodCode = payment.SelectedPaymentOption;

					if ((payment.CreditCardDetails != null) && paymentMethodCode != PaymentMethodCode.CreditCard)
					{
						payment.CreditCardDetails = null;

						foreach (var key in modelState.Keys.Where(k => k.Contains(PaymentDetailsKey)).ToList())
						{
							modelState.Remove(key);
						}
					}
				}
			}
		}

		public void OnActionExecuted(ActionExecutedContext context)
		{
		}
	}
}

using OnlineStore.Web.ViewModels.Checkout;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OnlineStore.Web.Infrastructure.Filters
{
	public class CleanEmptyAddressFilter : IActionFilter
	{
		private const string BillingAddressKey = "BillingAddress";
		private const string ShippingAddressKey = "ShippingAddress";

		public void OnActionExecuting(ActionExecutingContext context)
		{
			if (context.ActionArguments.TryGetValue("model", out var obj) 
					&& obj is CheckoutViewModel model)
			{

				var modelState = context.ModelState;

				if (model.GuestAddress != null)
				{
					if ((model.GuestAddress.BillingAddress != null) &&
							model.GuestAddress.BillingAddress.IsEmpty())
					{
						model.GuestAddress.BillingAddress = null;

						foreach (var key in modelState.Keys.Where(k => k.Contains(BillingAddressKey)).ToList())
						{
							modelState.Remove(key);
						}
					}
				}
				else if (model.MemberAddress != null)
				{
					if ((model.MemberAddress.NewBillingAddress != null) &&
							model.MemberAddress.NewBillingAddress.IsEmpty())
					{

						model.MemberAddress.NewBillingAddress = null;

						foreach (var key in modelState.Keys.Where(k => k.Contains(BillingAddressKey)).ToList())
						{
							modelState.Remove(key);
						}
					}
					else if ((model.MemberAddress.NewShippingAddress != null) &&
								model.MemberAddress.NewShippingAddress.IsEmpty())
					{
						model.MemberAddress.NewShippingAddress = null;

						foreach (var key in modelState.Keys.Where(k => k.Contains(ShippingAddressKey)).ToList())
						{
							modelState.Remove(key);
						}
					}
				}
			}
		}

		public void OnActionExecuted(ActionExecutedContext filterContext)
		{
			//Nothing to be done.
		}
	}
}

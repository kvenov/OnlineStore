﻿using OnlineStore.Data.Models;
using OnlineStore.Web.ViewModels.Checkout;

namespace OnlineStore.Services.Core.Interfaces
{
	public interface ICheckoutService
	{
		Task<Checkout?> InitializeCheckoutAsync(string? userId);

		Task<CheckoutViewModel?> GetUserCheckout(Checkout? checkout);
	}
}

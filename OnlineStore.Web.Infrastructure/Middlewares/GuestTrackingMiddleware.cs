﻿using Microsoft.AspNetCore.Http;
using System.Security.Claims;

using static OnlineStore.Common.ApplicationConstants;

namespace OnlineStore.Web.Infrastructure.Middlewares
{

	//This middleware is used to create identifier for guest users, in order to use it for guest cart functionallity.
	public class GuestTrackingMiddleware
	{
		private readonly RequestDelegate _next;

		public GuestTrackingMiddleware(RequestDelegate next)
		{
			this._next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			string guestIdentifier;

			if (context.User.Identity?.IsAuthenticated == true)
			{
				guestIdentifier = context.User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToString()!;
			}
			else
			{
				bool isCookieExist = context.Request.Cookies.TryGetValue(GuestCookieName, out guestIdentifier);

				if (!isCookieExist)
				{
					guestIdentifier = Guid.NewGuid().ToString();

					CookieOptions cookieOptions = new CookieOptions()
					{
						SameSite = SameSiteMode.Strict,
						HttpOnly = true,
						Secure = true,
						Expires = DateTimeOffset.UtcNow.AddDays(30)
					};

					context.Response.Cookies.Append(GuestCookieName, guestIdentifier, cookieOptions);
				}
			}

			context.Items["GuestIdentifier"] = guestIdentifier;

			await _next(context);
		}
	}
}

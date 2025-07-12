using Microsoft.AspNetCore.Builder;
using OnlineStore.Web.Infrastructure.Middlewares;

namespace OnlineStore.Web.Infrastructure.Extensions
{
	public static class WebApplicationExtensions
	{

		public static IApplicationBuilder UseGuestTracking(this IApplicationBuilder app)
		{

			app.UseMiddleware<GuestTrackingMiddleware>();

			return app;
		}

		public static IApplicationBuilder UseApiSafeStatusCodeRedirects(this IApplicationBuilder app)
		{

			app.UseMiddleware<ApiSafeStatusCodeRedirectMiddleware>();

			return app;
		}
	}
}

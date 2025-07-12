using Microsoft.AspNetCore.Http;

namespace OnlineStore.Web.Infrastructure.Middlewares
{

	//This middleware is used to redirect responses to custom error pages, only when the request is made to a browser routes(not api routes), and it contains text/html
	public class ApiSafeStatusCodeRedirectMiddleware
	{
		private const string ErrorRedirectPath = "/Home/Error";

		private readonly RequestDelegate _next;

		public ApiSafeStatusCodeRedirectMiddleware(RequestDelegate next)
		{
			this._next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{

			await _next(context);

			var path = context.Request.Path.Value;

			bool isApiRequest = path.StartsWith("/api", StringComparison.OrdinalIgnoreCase);
			bool expectsHtml = context.Request.Headers["Accept"].Any(h => h.Contains("text/html"));
			bool isErrorStatus = context.Response.StatusCode >= 400 && context.Response.StatusCode < 600;

			if (!isApiRequest && expectsHtml && isErrorStatus && context.Response.ContentLength == null && !context.Response.HasStarted)
			{
				var statusCode = context.Response.StatusCode;
				context.Response.Redirect($"{ErrorRedirectPath}?statusCode={statusCode}");
			}
		}
	}
}

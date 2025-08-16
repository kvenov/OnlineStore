using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using OnlineStore.Services.Core.Email.Interfaces;
using OnlineStore.Web.ViewModels.Email;

namespace OnlineStore.Services.Core.Email
{
	public class ViewRenderService : IViewRenderService
	{
		private readonly IRazorViewEngine _viewEngine;
		private readonly ITempDataProvider _tempDataProvider;
		private readonly IServiceProvider _serviceProvider;

		public ViewRenderService(IRazorViewEngine viewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
		{
			_viewEngine = viewEngine;
			_tempDataProvider = tempDataProvider;
			_serviceProvider = serviceProvider;
		}

		public async Task<string> RenderToStringAsync(string viewPath, OrderPlacedEmailModel model)
		{
			var actionContext = new ActionContext(new DefaultHttpContext { RequestServices = _serviceProvider }, new Microsoft.AspNetCore.Routing.RouteData(), new ActionDescriptor());
			using var sw = new StringWriter();
			var viewResult = _viewEngine.GetView(executingFilePath: null, viewPath: viewPath, isMainPage: true);

			if (!viewResult.Success)
			{
				throw new InvalidOperationException($"Email view '{viewPath}' not found.");
			}

			var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
			{
				Model = model
			};

			var tempData = new TempDataDictionary(actionContext.HttpContext, _tempDataProvider);

			var viewContext = new ViewContext(
				actionContext,
				viewResult.View,
				viewDictionary,
				tempData,
				sw,
				new HtmlHelperOptions()
			);

			await viewResult.View.RenderAsync(viewContext);
			return sw.ToString();
		}
	}
}

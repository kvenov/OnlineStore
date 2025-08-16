using OnlineStore.Web.ViewModels.Email;

namespace OnlineStore.Services.Core.Email.Interfaces
{
	public interface IViewRenderService
	{
		Task<string> RenderToStringAsync(string viewPath, OrderPlacedEmailModel model);
	}
}

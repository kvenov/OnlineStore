using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineStore.Web.ViewModels.Layout;

namespace OnlineStore.Services.Core.Interfaces
{
	public interface IProductCategoryService
	{
		Task<IEnumerable<SelectListItem>> GetAllProductCategoriesIdsAndNamesAsync();

		Task<IEnumerable<MenuViewModel>> GetLayoutCategoryMenuAsync();
	}
}

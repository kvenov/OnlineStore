using Microsoft.AspNetCore.Mvc.Rendering;

namespace OnlineStore.Services.Core.Admin.Interfaces
{
	public interface IAdminProductCategoryService
	{
		Task<IEnumerable<SelectListItem>> GetAllProductCategoriesIdsAndNamesAsync();
	}
}

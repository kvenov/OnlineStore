using Microsoft.AspNetCore.Mvc.Rendering;

namespace OnlineStore.Services.Core.Interfaces
{
	public interface IProductCategoryService
	{
		Task<IEnumerable<SelectListItem>> GetAllProductCategoriesIdsAndNamesAsync();
	}
}

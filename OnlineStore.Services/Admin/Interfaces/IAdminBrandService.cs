using Microsoft.AspNetCore.Mvc.Rendering;

namespace OnlineStore.Services.Core.Admin.Interfaces
{
	public interface IAdminBrandService
	{
		Task<IEnumerable<SelectListItem>> GetAllBrandsIdsAndNamesAsync();
	}
}

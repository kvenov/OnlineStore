using Microsoft.AspNetCore.Mvc.Rendering;

namespace OnlineStore.Services.Core.Interfaces
{
	public interface IBrandService
	{
		Task<IEnumerable<SelectListItem>> GetAllBrandsIdsAndNamesAsync();
	}
}

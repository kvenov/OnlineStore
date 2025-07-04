using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Services.Core.Interfaces;

namespace OnlineStore.Services.Core
{
	public class BrandService : IBrandService
	{
		private readonly IBrandRepository _repository;

		public BrandService(IBrandRepository repository)
		{
			this._repository = repository;
		}

		public async Task<IEnumerable<SelectListItem>> GetAllBrandsIdsAndNamesAsync()
		{
			IEnumerable<SelectListItem> brands = await this._repository
				.GetAllAttached()
				.AsNoTracking()
				.Select(c => new SelectListItem
				{
					Value = c.Id.ToString(),
					Text = c.Name
				})
				.ToListAsync();

			return brands;
		}
	}
}

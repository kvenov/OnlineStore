using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.Services.Core.Interfaces;

namespace OnlineStore.Services.Core
{
	public class BrandService : IBrandService
	{
		private readonly ApplicationDbContext _context;

		public BrandService(ApplicationDbContext context)
		{
			this._context = context;
		}

		public async Task<IEnumerable<SelectListItem>> GetAllBrandsIdsAndNamesAsync()
		{
			IEnumerable<SelectListItem> brands = await this._context
				.Brands
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

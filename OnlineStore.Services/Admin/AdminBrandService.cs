using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.Services.Core.Admin.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OnlineStore.Services.Core.Admin
{
	public class AdminBrandService : IAdminBrandService
	{
		private readonly ApplicationDbContext _context;

		public AdminBrandService(ApplicationDbContext context)
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

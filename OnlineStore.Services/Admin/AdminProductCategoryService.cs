using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.Services.Core.Admin.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OnlineStore.Services.Core.Admin
{
	public class AdminProductCategoryService : IAdminProductCategoryService
	{
		private readonly ApplicationDbContext _context;

		public AdminProductCategoryService(ApplicationDbContext context)
		{
			this._context = context;
		}

		public async Task<IEnumerable<SelectListItem>> GetAllProductCategoriesIdsAndNamesAsync()
		{
			IEnumerable<SelectListItem> productCategories = await this._context
				.ProductCategories
				.AsNoTracking()
				.Select(c => new SelectListItem
				{
					Value = c.Id.ToString(),
					Text = c.Name
				})
				.ToListAsync();

			return productCategories;
		}
	}
}

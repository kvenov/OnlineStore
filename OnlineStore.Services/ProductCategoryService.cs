using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.Services.Core.Interfaces;

namespace OnlineStore.Services.Core
{
	public class ProductCategoryService : IProductCategoryService
	{
		private readonly ApplicationDbContext _context;

		public ProductCategoryService(ApplicationDbContext context)
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

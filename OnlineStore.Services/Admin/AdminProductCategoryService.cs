using Microsoft.EntityFrameworkCore;
using OnlineStore.Services.Core.Admin.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineStore.Data.Repository.Interfaces;

namespace OnlineStore.Services.Core.Admin
{
	public class AdminProductCategoryService : IAdminProductCategoryService
	{
		private readonly IProductCategoryRepository _repository;

		public AdminProductCategoryService(IProductCategoryRepository repository)
		{
			this._repository = repository;
		}

		public async Task<IEnumerable<SelectListItem>> GetAllProductCategoriesIdsAndNamesAsync()
		{
			IEnumerable<SelectListItem> productCategories = await this._repository
				.GetAllAttached()
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

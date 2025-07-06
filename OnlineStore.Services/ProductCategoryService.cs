using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Services.Core.Interfaces;

namespace OnlineStore.Services.Core
{
	public class ProductCategoryService : IProductCategoryService
	{
		private readonly IRepository<ProductCategory, int> _repository;

		public ProductCategoryService(IRepository<ProductCategory, int> repository)
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

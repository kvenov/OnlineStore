using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.Layout;

using static OnlineStore.Data.Common.Constants.EntityConstants.ProductDetails;

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

		public async Task<IEnumerable<MenuViewModel>> GetLayoutCategoryMenuAsync()
		{

			var menuViewModel = new List<MenuViewModel>();

			foreach (var g in AllowedGenders)
			{
				var categories = await _repository
					.GetAllAttached()
					.Include(c => c.Subcategories)
					.Where(c => c.ParentCategoryId == null)
					.ToListAsync();

				var categoryGroups = categories.Select(c => new CategoryGroup
				{
					ParentCategory = c.Name,
					Subcategories = c.Subcategories.Select(sc => sc.Name).ToList()
				}).ToList();

				menuViewModel.Add(new MenuViewModel
				{
					Gender = g,
					CategoryGroups = categoryGroups
				});
			}


			return menuViewModel;
		}
	}
}

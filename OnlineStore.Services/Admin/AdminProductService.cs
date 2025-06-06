using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.Services.Core.Admin.Interfaces;
using OnlineStore.Web.ViewModels.Admin.Product;

namespace OnlineStore.Services.Core.Admin
{
	public class AdminProductService : IAdminProductService
	{
		private readonly ApplicationDbContext _context;

		public AdminProductService(ApplicationDbContext context)
		{
			this._context = context;
		}

		public async Task<IEnumerable<AllProductsViewModel>> GetAllProductsAsync()
		{
			IEnumerable<AllProductsViewModel> productList = await _context
				.Products
				.AsNoTracking()
				.Select(p => new AllProductsViewModel
				{
					Id = p.Id.ToString(),
					Name = p.Name,
					Description = p.Description,
					Price = p.Price.ToString("F2"),
					DiscountPrice = p.DiscountPrice.HasValue ? p.DiscountPrice.Value.ToString("F2") : null,
					Category = p.Category.Name,
					Brand = p.Brand != null ? p.Brand.Name : "",
					ImageUrl = p.ImageUrl
				})
				.ToListAsync();

			return productList;
		}
	}
}

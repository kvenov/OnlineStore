using OnlineStore.Data;
using OnlineStore.Services.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Web.ViewModels.Product;

namespace OnlineStore.Services.Core
{
	public class ProductService : IProductService
	{
		private readonly ApplicationDbContext _context;

		public ProductService(ApplicationDbContext context)
		{
			this._context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<IEnumerable<AllProductListViewModel>> GetAllProductsAsync()
		{
			IEnumerable<AllProductListViewModel> productList = await _context
			  .Products
			  .AsNoTracking()
			  .Select(p => new AllProductListViewModel
			  {
				  Id = p.Id,
				  Name = p.Name,
				  Description = p.Description,
				  Price = p.Price,
				  ImageUrl = p.ImageUrl
			  })
			  .ToListAsync();

			return productList;
		}

		public async Task<AllProductListViewModel> GetProductByIdAsync(int id)
		{
			return await _context
				.Products
				.AsNoTracking()
				.Select(p => new AllProductListViewModel
				{
					Id = p.Id,
					Name = p.Name,
					Description = p.Description,
					Price = p.Price,
					ImageUrl = p.ImageUrl
				})
				.FirstAsync(p => p.Id == id);
		}

	}
}

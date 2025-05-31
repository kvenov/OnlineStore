using OnlineStore.Data;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace OnlineStore.Services.Core
{
	public class ProductService(ApplicationDbContext context) : IProductService
	{

		public async Task<IEnumerable<ProductViewModel>> GetAllProductsAsync()
		{
			return await context
			  .Products
			  .AsNoTracking()
			  .Select(p => new ProductViewModel
			  {
				  Id = p.Id,
				  Name = p.Name,
				  Description = p.Description,
				  Price = p.Price,
				  ImageUrl = p.ImageUrl
			  })
			  .ToListAsync();
		}

		public async Task<ProductViewModel> GetProductByIdAsync(int id)
		{
			return await context
				.Products
				.AsNoTracking()
				.Select(p => new ProductViewModel
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

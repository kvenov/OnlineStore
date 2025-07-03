using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.Home.Partial;

namespace OnlineStore.Services.Core
{
	public class ProductPromotionService : IProductPromotionService
	{
		private readonly ApplicationDbContext _context;

		public ProductPromotionService(ApplicationDbContext context)
		{
			this._context = context;
		}

		public async Task<IEnumerable<ProductPromotionViewModel>> GetProductsPromotionsAsync()
		{
			IEnumerable<ProductPromotionViewModel> promotions = await this._context
								.ProductsPromotions
								.AsNoTracking()
								.Include(p => p.Product)
								.Select(p => new ProductPromotionViewModel()
								{
									ProductId = p.ProductId,
									ImageUrl = p.Product.ImageUrl,
									ProductName = p.Product.Name,
									Label = p.Label,
									ExpDate = p.ExpDate.ToString("yyyy-MM-dd"),
									Percent = Math
													.Round((((p.Product.Price - p.Product.DiscountPrice) / p.Product.Price) * 100)!.Value)
														.ToString()
															.TrimEnd('0')
																.TrimEnd('.')
								})
								.ToListAsync();

			return promotions;
		}
	}
}

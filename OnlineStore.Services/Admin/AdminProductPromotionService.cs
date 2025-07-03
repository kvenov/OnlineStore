using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.Services.Core.Admin.Interfaces;
using OnlineStore.Web.ViewModels.Admin.ProductPromotion;

namespace OnlineStore.Services.Core.Admin
{
	public class AdminProductPromotionService : IAdminProductPromotionService
	{
		private readonly ApplicationDbContext _context;

		public AdminProductPromotionService(ApplicationDbContext context)
		{
			this._context = context;
		}

		public async Task<IEnumerable<PromotionIndexViewModel>> GetProductsPromotionsAsync()
		{
			IEnumerable<PromotionIndexViewModel> promotions = await this._context
							.ProductsPromotions
							.IgnoreQueryFilters()
							.AsNoTracking()
							.Include(p => p.Product)
							.Select(p => new PromotionIndexViewModel()
							{
								Id = p.Id,
								PromotionPrice = p.PromotionPrice,
								ProductName = p.Product.Name,
								Label = p.Label,
								StartDate = p.StartDate,
								ExpDate = p.ExpDate,
								IsDeleted = p.IsDeleted
							})
							.ToListAsync();

			return promotions;
		}
	}
}

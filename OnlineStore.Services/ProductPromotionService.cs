using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.Home.Partial;

namespace OnlineStore.Services.Core
{
	public class ProductPromotionService : IProductPromotionService
	{
		private readonly IProductPromotionRepository _productPromotionRepository;

		public ProductPromotionService(IProductPromotionRepository productPromotionRepository)
		{
			this._productPromotionRepository = productPromotionRepository;
		}

		public async Task<IEnumerable<ProductPromotionViewModel>> GetProductsPromotionsAsync()
		{
			DateTime today = DateTime.Now;
			IEnumerable<ProductPromotionViewModel> promotions = await this._productPromotionRepository
								.GetAllAttached()
								.AsNoTracking()
								.Where(p => p.IsDeleted == false && p.ExpDate > today)
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

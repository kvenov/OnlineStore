using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.Data.Models;
using OnlineStore.Services.Core.Admin.Interfaces;
using OnlineStore.Web.ViewModels.Admin.ProductPromotion;
using System.Globalization;

namespace OnlineStore.Services.Core.Admin
{
	public class AdminProductPromotionService : IAdminProductPromotionService
	{
		private readonly ApplicationDbContext _context;

		public AdminProductPromotionService(ApplicationDbContext context)
		{
			this._context = context;
		}

		public async Task<bool> CreateProductPromotion(AddPromotionInputModel? model)
		{
			bool isCreated = false;

			if (model != null)
			{

				Product? product = await this._context
						.Products
						.FindAsync(model.ProductId);

				bool isPromotionPriceValid = decimal.TryParse(model.PromotionPrice, out var promotionPrice);

				bool isStartDateValid = DateTime.TryParse(model.StartDate, CultureInfo.InvariantCulture, out var startDate);
				bool isExpDateValid = DateTime.TryParse(model.ExpDate, CultureInfo.InvariantCulture, out var expDate);


				bool isDeletedValid = bool.TryParse(model.IsDeleted, out var isDeleted);

				if ((product != null) && (isPromotionPriceValid) && 
				    (isStartDateValid) && (isExpDateValid) && (isDeletedValid))
				{
					bool isPromotionValid = await this._context
						.ProductsPromotions
						.AsNoTracking()
						.Where(p => p.IsDeleted == false)
						.Include(p => p.Product)
						.AnyAsync(p => p.Product.Name == product.Name && p.PromotionPrice == promotionPrice);

					if (!isPromotionValid)
					{
						ProductPromotion promotion = new ProductPromotion()
						{
							ProductId = product.Id,
							PromotionPrice = promotionPrice,
							Label = model.Label,
							StartDate = startDate,
							ExpDate = expDate,
							IsDeleted = isDeleted,
						};

						product.DiscountPrice = promotion.PromotionPrice;

						await this._context.ProductsPromotions.AddAsync(promotion);
						int affectedRows = await this._context.SaveChangesAsync();

						isCreated = affectedRows > 0;
					}
				}
			}

			return isCreated;
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

using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Services.Core.Admin.Interfaces;
using OnlineStore.Web.ViewModels.Admin.ProductPromotion;
using System.Globalization;

namespace OnlineStore.Services.Core.Admin
{
	public class AdminProductPromotionService : IAdminProductPromotionService
	{
		private readonly IProductRepository _productRepository;
		private readonly IProductPromotionRepository _productPromotionRepository;

		public AdminProductPromotionService(IProductRepository productRepository, IProductPromotionRepository productPromotionRepository)
		{
			this._productRepository = productRepository;
			this._productPromotionRepository = productPromotionRepository;
		}

		public async Task<bool> CreateProductPromotion(AddPromotionInputModel? model)
		{
			bool isCreated = false;

			if (model != null)
			{

				Product? product = await this._productRepository
					.GetByIdAsync(model.ProductId);

				bool isPromotionPriceValid = decimal.TryParse(model.PromotionPrice, out var promotionPrice);

				bool isStartDateValid = DateTime.TryParse(model.StartDate, CultureInfo.InvariantCulture, out var startDate);
				bool isExpDateValid = DateTime.TryParse(model.ExpDate, CultureInfo.InvariantCulture, out var expDate);


				bool isDeletedValid = bool.TryParse(model.IsDeleted, out var isDeleted);

				if ((product != null) && (isPromotionPriceValid) && 
				    (isStartDateValid) && (isExpDateValid) && (isDeletedValid))
				{
					bool isPromotionValid = await this._productPromotionRepository
						.GetAllAttached()
						.AsNoTracking()
						.Where(p => p.IsDeleted == false)
						.Include(p => p.Product)
						.AnyAsync(p => p.Product.Name == product.Name && p.PromotionPrice == promotionPrice);

					bool isDateRangeValid = startDate < expDate;

					if ((!isPromotionValid) && (isDateRangeValid))
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

						await this._productPromotionRepository.AddAsync(promotion);
						isCreated = true;
					}
				}
			}

			return isCreated;
		}

		public async Task<bool> DeletePromotionAsync(int? promotionId)
		{
			bool isDeleted = false;

			if (promotionId != null)
			{
				ProductPromotion? promotionToDelete = await this._productPromotionRepository
						.GetByIdAsync(promotionId.Value);

				if (promotionToDelete != null)
				{
					Product? product = await this._productRepository
								.SingleOrDefaultAsync(p => p.Id == promotionToDelete.ProductId);

					if (product != null)
					{
						product.DiscountPrice = product.Price;
					}

					isDeleted = await this._productPromotionRepository.DeleteAsync(promotionToDelete);
				}
			}

			return isDeleted;
		}

		public async Task<bool> EditPromotionAsync(EditPromotionInputModel? model)
		{
			bool isEdited = false;

			if (model != null)
			{

				Product? product = await this._productRepository
						.GetByIdAsync(model.ProductId);

				bool isPromotionPriceValid = decimal.TryParse(model.PromotionPrice, out var promotionPrice);

				bool isStartDateValid = DateTime.TryParse(model.StartDate, CultureInfo.InvariantCulture, out var startDate);
				bool isExpDateValid = DateTime.TryParse(model.ExpDate, CultureInfo.InvariantCulture, out var expDate);


				bool isDeletedValid = bool.TryParse(model.IsDeleted, out var isDeleted);

				if ((product != null) && (isPromotionPriceValid) &&
					(isStartDateValid) && (isExpDateValid) && (isDeletedValid))
				{
					bool isDateRangeValid = startDate < expDate;

					if (isDateRangeValid)
					{
						ProductPromotion? promotionToEdit = await this._productPromotionRepository
							.GetAllAttached()
							.Include(p => p.Product)
							.SingleOrDefaultAsync(p => p.Id == model.Id);

						if (promotionToEdit != null)
						{
							promotionToEdit.ProductId = product.Id;
							promotionToEdit.PromotionPrice = promotionPrice;
							promotionToEdit.Label = model.Label;
							promotionToEdit.StartDate = startDate;
							promotionToEdit.ExpDate = expDate;
							promotionToEdit.IsDeleted = isDeleted;

							product.DiscountPrice = promotionToEdit.PromotionPrice;

							isEdited = await this._productPromotionRepository.UpdateAsync(promotionToEdit);
						}
					}
				}
			}

			return isEdited;
		}

		public async Task<IEnumerable<PromotionIndexViewModel>> GetProductsPromotionsAsync()
		{
			IEnumerable<PromotionIndexViewModel> promotions = await this._productPromotionRepository
							.GetAllAttached()
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

		public async Task<PromotionGetViewModel?> GetPromotionByIdAsync(int? promotionId)
		{
			PromotionGetViewModel? promotionModel = null;

			if (promotionId != null)
			{
				ProductPromotion? promotion = await this._productPromotionRepository
					.GetAllAttached()
					.AsNoTracking()
					.Include(p => p.Product)
					.SingleOrDefaultAsync(p => p.Id == promotionId);

				if (promotion != null)
				{
					promotionModel = new PromotionGetViewModel()
					{
						Id = promotion.Id,
						ProductId = promotion.ProductId,
						Label = promotion.Label,
						PromotionPrice = promotion.PromotionPrice.ToString("F2"),
						StartDate = promotion.StartDate.ToString("yyyy-MM-dd"),
						ExpDate = promotion.ExpDate.ToString("yyyy-MM-dd"),
						IsDeleted = promotion.IsDeleted,
					};
				}
			}

			return promotionModel;
		}
	}
}

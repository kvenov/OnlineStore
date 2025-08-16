using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Services.Core.Interfaces;

namespace OnlineStore.Services.Core
{
	public class ProductRatingService : IProductRatingService
	{
		private readonly IProductRepository _productRepository;

		public ProductRatingService(IProductRepository productRepository)
		{
			this._productRepository = productRepository;
		}

		public async Task RecalculateProductRatingAsync(int productId)
		{
			var product = await this._productRepository
				.FirstOrDefaultAsync(p => p.Id == productId);

			if (product == null)
				return;

			var ratings = product.ProductRatings
				.Where(r => !r.IsDeleted)
				.Select(r => r.Rating)
				.ToList();

			var ordersCount = product.OrderItems
					.Select(oi => oi.OrderId)
					.Distinct()
					.Count();

			var missingRatings = ordersCount - ratings.Count;
			for (int i = 0; i < missingRatings; i++)
				ratings.Add(1);

			product.TotalRatings = ratings.Count;
			product.AverageRating = ratings.Count == 0 ? 0 : ratings.Average();

			await this._productRepository.UpdateAsync(product);
		}
	}
}

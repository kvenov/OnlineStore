namespace OnlineStore.Services.Core.Interfaces
{
	public interface IProductRatingService
	{
		Task RecalculateProductRatingAsync(int productId);
	}
}

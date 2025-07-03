using OnlineStore.Web.ViewModels.Home.Partial;

namespace OnlineStore.Web.ViewModels.Home
{
	public class HomeIndexViewModel
	{

		public IEnumerable<ProductPromotionViewModel> Promotions { get; set; } = 
					new List<ProductPromotionViewModel>();

		public IEnumerable<TrendingProductViewModel> Trendings { get; set; } = 
					new List<TrendingProductViewModel>();

		public IEnumerable<ProductNewArrivalViewModel> NewArrivals { get; set; } =
					new List<ProductNewArrivalViewModel>();

		public IEnumerable<UserReviewViewModel> Reviews { get; set; } =
					new List<UserReviewViewModel>();
	}
}

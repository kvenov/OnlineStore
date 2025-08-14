using OnlineStore.Web.ViewModels.Product.Partial;

namespace OnlineStore.Web.ViewModels.Product
{
	public class SearchProductListViewModel
	{
		public IEnumerable<SearchProductListItemViewModel> Products { get; set; } = new List<SearchProductListItemViewModel>();

	}
}

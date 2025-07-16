namespace OnlineStore.Web.ViewModels.Product
{
	public class SearchProductListViewModel
	{
		public IEnumerable<AllProductListViewModel> Products { get; set; } = new List<AllProductListViewModel>();

		public IEnumerable<string> SubCategories { get; set; } = new List<string>();
	}
}

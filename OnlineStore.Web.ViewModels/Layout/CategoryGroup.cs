namespace OnlineStore.Web.ViewModels.Layout
{
	public class CategoryGroup
	{
		public string ParentCategory { get; set; } = null!;
		public List<string> Subcategories { get; set; } = new();
	}
}

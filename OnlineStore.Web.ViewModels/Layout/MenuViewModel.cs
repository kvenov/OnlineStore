namespace OnlineStore.Web.ViewModels.Layout
{
	public class MenuViewModel
	{

		public string Gender { get; set; } = null!;
		public List<CategoryGroup> CategoryGroups { get; set; } = new();
	}
}

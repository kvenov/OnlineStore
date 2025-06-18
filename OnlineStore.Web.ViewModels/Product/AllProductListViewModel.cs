namespace OnlineStore.Web.ViewModels.Product
{
	public class AllProductListViewModel
	{

		public int Id { get; set; }

		public string Name { get; set; } = null!;

		public string Description { get; set; } = null!;

		public decimal Price { get; set; }

		public string ImageUrl { get; set; } = null!;

		public float Rating { get; set; }
	}
}

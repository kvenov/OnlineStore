using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineStore.Web.ViewModels.Product;

namespace OnlineStore.Web.ViewModels.Home
{
	public class HomeIndexViewModel
	{

		public IEnumerable<AllProductListViewModel> Products { get; set; } = new List<AllProductListViewModel>();
		public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
		public IEnumerable<SelectListItem> Brands { get; set; } = new List<SelectListItem>();

		public int CategoryId { get; set; }
		public int? BrandId { get; set; }
	}
}

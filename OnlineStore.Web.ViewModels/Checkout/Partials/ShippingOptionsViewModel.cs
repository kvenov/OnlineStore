using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Web.ViewModels.Checkout.Partials
{
	public class ShippingOptionsViewModel
	{

		[Required]
		public ShippingOptionItemViewModel SelectedShippingOption { get; set; } = new();

		public List<ShippingOptionItemViewModel>? Options { get; set; }
	}
}

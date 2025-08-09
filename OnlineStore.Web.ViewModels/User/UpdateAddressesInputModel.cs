using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Web.ViewModels.User
{
	public class UpdateAddressesInputModel
	{

		[Required(ErrorMessage = "Please select a default shipping address.")]
		public int DefaultShippingAddressId { get; set; }

		[Required(ErrorMessage = "Please select a default billing address.")]
		public int DefaultBillingAddressId { get; set; }
	}
}

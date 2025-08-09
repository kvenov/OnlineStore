using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Web.ViewModels.User
{
	public class UpdatePaymentInputModel
	{

		[Required(ErrorMessage = "Please select a default payment method.")]
		public int DefaultPaymentMethodId { get; set; }

		[Required(ErrorMessage = "Please select default payment details.")]
		public int DefaultPaymentDetailsId { get; set; }
	}
}

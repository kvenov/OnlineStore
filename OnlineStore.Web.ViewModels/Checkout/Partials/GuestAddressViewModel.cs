using System.ComponentModel.DataAnnotations;

using static OnlineStore.Common.ApplicationConstants.AddressValidationConstants;
using static OnlineStore.Common.ErrorMessages.AddressValidationMessages;

namespace OnlineStore.Web.ViewModels.Checkout.Partials
{
	public class GuestAddressViewModel
	{

		[Required(ErrorMessage = Required)]
		[StringLength(FullNameMaxLength, MinimumLength = FullNameMinLength, ErrorMessage = FullNameLength)]
		public string FullName { get; set; } = null!;

		[Required(ErrorMessage = Required)]
		[EmailAddress(ErrorMessage = EmailInvalid)]
		[StringLength(EmailMaxLength, ErrorMessage = EmailLength)]
		public string Email { get; set; } = null!;
	
		[Required]
		public GuestShippingAddressViewModel ShippingAddress { get; set; } = new();
		public GuestBillingAddressViewModel? BillingAddress { get; set; }

	}
}

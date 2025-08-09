using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Web.ViewModels.User
{
	public class AccountSettingsViewModel
	{

		[StringLength(100)]
		[Display(Name = "Full Name")]
		public string? FullName { get; set; }

		[StringLength(100)]
		[Display(Name = "Username")]
		public string? Username { get; set; }

		[Required, EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; } = null!;

		[Phone]
		[Display(Name = "Phone Number")]
		public string? PhoneNumber { get; set; }

		public ChangePasswordInputModel ChangePassword { get; set; } = new();

		public UpdateAddressesInputModel Addresses { get; set; } = new();

		public UpdatePaymentInputModel Payment { get; set; } = new();


		public IEnumerable<AddressDropdownViewModel> AvailableAddresses { get; set; } = new List<AddressDropdownViewModel>();

		public IEnumerable<PaymentMethodDropdownViewModel> AvailablePaymentMethods { get; set; } = new List<PaymentMethodDropdownViewModel>();
		public IEnumerable<PaymentDetailsDropdownViewModel> AvailablePaymentDetails { get; set; } = new List<PaymentDetailsDropdownViewModel>();
	}
}

namespace OnlineStore.Web.ViewModels.User
{
	public class AccountSettingsViewModel
	{

		public UpdateProfileInputModel Profile { get; set; } = new();

		public ChangePasswordInputModel ChangePassword { get; set; } = new();

		public UpdateAddressesInputModel Addresses { get; set; } = new();

		public UpdatePaymentInputModel Payment { get; set; } = new();


		public IEnumerable<AddressDropdownViewModel> AvailableAddresses { get; set; } = new List<AddressDropdownViewModel>();

		public IEnumerable<PaymentMethodDropdownViewModel> AvailablePaymentMethods { get; set; } = new List<PaymentMethodDropdownViewModel>();
		public IEnumerable<PaymentDetailsDropdownViewModel> AvailablePaymentDetails { get; set; } = new List<PaymentDetailsDropdownViewModel>();
	}
}

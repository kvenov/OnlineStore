using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models;
using OnlineStore.Data.Models.Enums;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.User;
using System.Text;

namespace OnlineStore.Services.Core
{
	public class UserService : IUserService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IRepository<PaymentMethod, int> _paymentMethodRepository;
		private readonly IRepository<PaymentDetails, int> _paymentDetailsRepository;

		public UserService(UserManager<ApplicationUser> userManager, 
						   RoleManager<IdentityRole> roleManager, 
						   IRepository<PaymentMethod, int> paymentMethodRepository,
						   IRepository<PaymentDetails, int> paymentDetailsRepository)
		{
			this._userManager = userManager;
			this._roleManager = roleManager;
			this._paymentMethodRepository = paymentMethodRepository;
			this._paymentDetailsRepository = paymentDetailsRepository;
		}

		public async Task<AccountSettingsViewModel?> GetUserSettingsAsync(string? userId)
		{
			AccountSettingsViewModel? model = null;

			if (!string.IsNullOrWhiteSpace(userId))
			{
				ApplicationUser? user = await this._userManager
									.FindByIdAsync(userId);

				if (user != null)
				{
					model = new()
					{
						FullName = user.FullName,
						Username = user.UserName,
						Email = user.Email!,
						PhoneNumber = user.PhoneNumber,
						Addresses = new UpdateAddressesInputModel()
						{
							DefaultShippingAddressId = user.DefaultShippingAddressId ?? 0,
							DefaultBillingAddressId = user.DefaultBillingAddressId ?? 0,
						},
						AvailableAddresses = user.Addresses
												.Select(a => new AddressDropdownViewModel()
												{
													DisplayName = FormatAddress(a),
													Id = a.Id
												}),
						Payment = new UpdatePaymentInputModel()
						{
							DefaultPaymentMethodId = user.DefaultPaymentMethodId ?? 0,
							DefaultPaymentDetailsId = user.DefaultPaymentDetailsId ?? 0,
						},
						AvailablePaymentMethods = await GetAvailablePaymentMethodsAsync(),
						AvailablePaymentDetails = await GetAvailablePaymentDetailsAsync(),
					};
				}
			}

			return model;
		}


		private static string FormatAddress(Address address)
		{
			StringBuilder str = new StringBuilder();

			string streetAddress = address.Street.ToString();
			string city = address.City.ToString();
			string zipCode = address.ZipCode.ToString();
			string country = address.Country.ToString();
			string phoneNumber = address.PhoneNumber.ToString();

			str.AppendLine(streetAddress);

			str.AppendLine($"{city}, {zipCode}");

			str.AppendLine(country);

			str.Append($"Phone: {phoneNumber}");

			return str.ToString().TrimEnd();
		}

		private static string FormatPaymentDetails(PaymentDetails details)
		{
			StringBuilder str = new StringBuilder();

			string checkoutCardNumber = details.CardNumber;

			string maskedCardNumber = checkoutCardNumber.Length > 4
				? new string('*', checkoutCardNumber.Length - 4) + checkoutCardNumber.Substring(checkoutCardNumber.Length - 4)
				: checkoutCardNumber;

			string nameOnCard = details.NameOnCard;
			int expMonth = details.ExpMonth;
			int expYear = details.ExpYear;

			str.AppendLine(nameOnCard);
			str.AppendLine($"{expMonth}/{expYear}");
			str.AppendLine(maskedCardNumber);

			return str.ToString().TrimEnd();
		}

		private async Task<List<PaymentMethodDropdownViewModel>> GetAvailablePaymentMethodsAsync()
		{
			var availablePaymentMethods = await this._paymentMethodRepository
									.GetAllAttached()
									.AsNoTracking()
									.Where(pm => pm.Code == PaymentMethodCode.CreditCard || pm.Code == PaymentMethodCode.PayPal || 
												 pm.Code == PaymentMethodCode.GooglePay || pm.Code == PaymentMethodCode.CashOnDelivery)
									.Select(pm => new PaymentMethodDropdownViewModel()
									{
										Id = pm.Id,
										MethodName = pm.Name
									})
									.ToListAsync();

			return availablePaymentMethods;
		}

		private async Task<List<PaymentDetailsDropdownViewModel>> GetAvailablePaymentDetailsAsync()
		{
			var availablePaymentDetails = await this._paymentDetailsRepository
									.GetAllAttached()
									.AsNoTracking()
									.Select(pd => new PaymentDetailsDropdownViewModel()
									{
										Id = pd.Id,
										CardSummary = FormatPaymentDetails(pd)
									})
									.ToListAsync();

			return availablePaymentDetails; ;
		}
	}
}

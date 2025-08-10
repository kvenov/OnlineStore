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
		private readonly IRepository<PaymentMethod, int> _paymentMethodRepository;
		private readonly IRepository<PaymentDetails, int> _paymentDetailsRepository;

		public UserService(UserManager<ApplicationUser> userManager,
						   IRepository<PaymentMethod, int> paymentMethodRepository,
						   IRepository<PaymentDetails, int> paymentDetailsRepository)
		{
			this._userManager = userManager;
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
						Profile = new UpdateProfileInputModel()
						{
							FullName = user.FullName,
							Username = user.UserName,
							Email = user.Email,
							PhoneNumber = user.PhoneNumber,
						},
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

		public async Task<(bool result, string message)> ChangeUserProfileAsync(UpdateProfileInputModel? model, string? userId)
		{
			if (model != null && !string.IsNullOrWhiteSpace(userId))
			{
				ApplicationUser? user = await _userManager
										.FindByIdAsync(userId);

				if (user != null)
				{
					bool isUserFullNameTheSame = user.FullName != null && user.FullName
									.Equals(model.FullName, StringComparison.OrdinalIgnoreCase);

					bool isUsernameTheSame = user.UserName != null && user.UserName
									.Equals(model.Username, StringComparison.OrdinalIgnoreCase);

					bool isUserEmailTheSame = user.Email != null && user.Email
									.Equals(model.Email, StringComparison.OrdinalIgnoreCase);

					bool isUserPhoneNumberTheSame = user.PhoneNumber != null && user.PhoneNumber
									.Equals(model.PhoneNumber, StringComparison.OrdinalIgnoreCase);

					if (!isUserFullNameTheSame) user.FullName = model.FullName;
					if (!isUsernameTheSame) user.UserName = model.Username;
					if (!isUserEmailTheSame) user.Email = model.Email;
					if (!isUserPhoneNumberTheSame) user.PhoneNumber = model.PhoneNumber;


					if (!isUserFullNameTheSame || !isUsernameTheSame || !isUserEmailTheSame || !isUserPhoneNumberTheSame)
					{
						await this._userManager.UpdateAsync(user);
						return (true, "The User profile is successfully updated!");
					}
					else
					{
						return (false, "Nothing new to change on your profile!");
					}
				}

			}

			return (false, "Something went wrong!");
		}

		public async Task<(bool result, string message)> ChangeUserPasswordAsync(ChangePasswordInputModel? model, string? userId)
		{
			if (model != null && !string.IsNullOrWhiteSpace(userId))
			{

				bool isParamsValid = !string.IsNullOrWhiteSpace(model.CurrentPassword) &&
											!string.IsNullOrWhiteSpace(model.NewPassword) &&
												!string.IsNullOrWhiteSpace(model.ConfirmNewPassword);

				bool isNewPasswordValid = model.NewPassword
									.Equals(model.ConfirmNewPassword);

				if (isParamsValid && isNewPasswordValid)
				{
					ApplicationUser? user = await _userManager
											.FindByIdAsync(userId);

					if (user != null)
					{
						bool isUserPasswordTheSameAsNew = model.CurrentPassword
																.Equals(model.NewPassword, StringComparison.OrdinalIgnoreCase);
						if (!isUserPasswordTheSameAsNew)
						{
							var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

							if (result.Succeeded)
							{
								return (true, "Your password is successfully changed!");
							}
						}
						else
						{
							return (false, "The passwords are idetified as same!");
						}
					}
				}
			}

			return (false, "Something went wrong!");
		}

		public async Task<(bool result, string message)> ChangeUserAddressesAsync(UpdateAddressesInputModel? model, string? userId)
		{
			if (model != null && !string.IsNullOrWhiteSpace(userId))
			{
				ApplicationUser? user = await _userManager
										.FindByIdAsync(userId);

				if (user != null)
				{
					int? shippingId = model.DefaultShippingAddressId == 0 ? null : model.DefaultShippingAddressId;
					int? billingId = model.DefaultBillingAddressId == 0 ? null : model.DefaultBillingAddressId;

					bool isUserDefaultShippingTheSame = user.DefaultShippingAddressId != null &&
															user.DefaultShippingAddressId == shippingId;

					bool isUserDefaultBillingTheSame = user.DefaultBillingAddressId != null &&
															user.DefaultBillingAddressId == billingId;

					int shouldUserUpdate = 0;
					if (!isUserDefaultShippingTheSame)
					{
						if (user.DefaultShippingAddressId != null || shippingId != null)
						{
							user.DefaultShippingAddressId = shippingId;
							shouldUserUpdate++;
						}
					}
					if (!isUserDefaultBillingTheSame) 
					{ 
						if (user.DefaultBillingAddressId != null || billingId != null)
						{
							user.DefaultBillingAddressId = billingId;
							shouldUserUpdate++;
						}
					} 

					if (shouldUserUpdate > 0)
					{
						await this._userManager.UpdateAsync(user);
						return (true, "The User default addresses are successfully updated!");
					}
					else
					{
						return (false, "The User has already these addresses as defaults!");
					}
				}
			}

			return (false, "Something went wrong!");
		}

		public async Task<(bool result, string message)> ChangeUserPaymentDataAsync(UpdatePaymentInputModel? model, string? userId)
		{
			if (model != null && !string.IsNullOrWhiteSpace(userId))
			{
				ApplicationUser? user = await _userManager
										.FindByIdAsync(userId);

				if (user != null)
				{
					int? methodId = model.DefaultPaymentMethodId == 0 ? null : model.DefaultPaymentMethodId;
					int? detailsId = model.DefaultPaymentDetailsId == 0 ? null : model.DefaultPaymentDetailsId;

					bool isUserDefaultMethodTheSame = user.DefaultPaymentMethodId != null &&
															user.DefaultPaymentMethodId == methodId;

					bool isUserDefaultDetailsTheSame = user.DefaultPaymentDetailsId != null &&
															user.DefaultPaymentDetailsId == detailsId;

					int shouldUserUpdate = 0;
					if (!isUserDefaultMethodTheSame)
					{
						if (user.DefaultPaymentMethodId != null || methodId != null)
						{
							user.DefaultPaymentMethodId = methodId;
							shouldUserUpdate++;
						}
					}

					if (!isUserDefaultDetailsTheSame)
					{
						if (user.DefaultPaymentDetailsId != null || detailsId != null)
						{
							user.DefaultPaymentDetailsId = detailsId;
							shouldUserUpdate++;
						}
					}

					if (shouldUserUpdate > 0)
					{
						await this._userManager.UpdateAsync(user);
						return (true, "The User default payment data is successfully updated!");
					}
					else
					{
						return (false, "The User has already this payment data as default!");
					}
				}
				
			}

			return (false, "Something went wrong!");
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

using Microsoft.AspNetCore.Identity;
using OnlineStore.Data.Models;
using OnlineStore.Data.Models.Enums;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.Checkout;
using OnlineStore.Web.ViewModels.Checkout.Partials;

using static OnlineStore.Common.ApplicationConstants;
using static OnlineStore.Services.Common.ServiceConstants;
using static OnlineStore.Common.ApplicationConstants.CreditCardValidationConstants;

namespace OnlineStore.Services.Core
{
	public class CheckoutService : ICheckoutService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IShoppingCartRepository _shoppingCartRepository;
		private readonly ICheckoutRepository _checkoutRepository;
		private readonly IAsyncRepository<PaymentMethod, int> _paymentMethodRepository;
		private readonly IAsyncRepository<PaymentDetails, int> _paymentDetailsRepository;
		private readonly IAsyncRepository<Address, int> _addressRepository;

		public CheckoutService(UserManager<ApplicationUser> userManager, 
							   IShoppingCartRepository shoppingCartRepository, 
							   ICheckoutRepository checkoutRepository,
							   IAsyncRepository<PaymentMethod, int> paymentMethodRepository,
							   IAsyncRepository<PaymentDetails, int> paymentDetailsRepository,
							   IAsyncRepository<Address, int> addressRepository)
		{
			this._userManager = userManager;
			this._shoppingCartRepository = shoppingCartRepository;
			this._checkoutRepository = checkoutRepository;
			this._paymentMethodRepository = paymentMethodRepository;
			this._paymentDetailsRepository = paymentDetailsRepository;
			this._addressRepository = addressRepository;
		}


		public async Task<Checkout?> InitializeCheckoutAsync(string? userId)
		{
			Checkout? checkout = null;
			if (userId != null)
			{
				ApplicationUser? user = await _userManager
							.FindByIdAsync(userId);

				if (user != null)
				{
					var existingCheckout = await this._checkoutRepository
								.SingleOrDefaultAsync(c => c.UserId == user.Id && 
														   c.Order == null && 
														   c.CompletedAt == null);

					if (existingCheckout != null)
					{
						await this._checkoutRepository.RefreshCheckoutStartingDateAsync(existingCheckout.Id);
						await this._checkoutRepository.UpdateCheckoutFromUserDefaultsAsync(existingCheckout.Id, user);
						await this._checkoutRepository.UpdateCheckoutFromLastOrderAsync(existingCheckout.Id, user.Id);
						await this._checkoutRepository.RefreshCheckoutTotalsAsync(userId, existingCheckout.Id);

						return existingCheckout;
					}

					var shoppingCart = await this._shoppingCartRepository
									.SingleOrDefaultAsync(sc => sc.UserId == user.Id);

					decimal subTotal = await this._shoppingCartRepository
									.GetItemsTotalPrice(userId);

					var newCheckout = new Checkout
					{
						UserId = user.Id,
						ShoppingCartId = shoppingCart!.Id,
						StartedAt = DateTime.UtcNow,
						SubTotal = subTotal
					};

					await this._checkoutRepository.SetCheckoutDefaultPaymentMethodAsync(newCheckout);
					this._checkoutRepository.SetCheckoutDefaultShippingOption(newCheckout, user);

					await this._checkoutRepository.AddAsync(newCheckout);

					await this._checkoutRepository.UpdateCheckoutFromUserDefaultsAsync(newCheckout.Id, user);
					await this._checkoutRepository.UpdateCheckoutFromLastOrderAsync(newCheckout.Id, user.Id);

					checkout = newCheckout;
				}
				else
				{
					var existingCheckout = await this._checkoutRepository
							.SingleOrDefaultAsync(c =>
									c.GuestId == userId &&
									c.Order == null && 
									c.CompletedAt == null);

					if (existingCheckout != null)
					{
						await this._checkoutRepository.RefreshCheckoutStartingDateAsync(existingCheckout.Id);
						await this._checkoutRepository.UpdateCheckoutFromLastOrderAsync(existingCheckout.Id, userId);
						await this._checkoutRepository.RefreshCheckoutTotalsAsync(userId, existingCheckout.Id);

						return existingCheckout;
					}

					var shoppingCart = await this._shoppingCartRepository
							.SingleOrDefaultAsync(sc => sc.GuestId == userId);

					decimal subTotal = await this._shoppingCartRepository
									.GetItemsTotalPrice(userId);

					var newCheckout = new Checkout
					{
						GuestId = userId,
						ShoppingCartId = shoppingCart!.Id,
						StartedAt = DateTime.UtcNow,
						SubTotal = subTotal
					};

					await this._checkoutRepository.SetCheckoutDefaultPaymentMethodAsync(newCheckout);
					this._checkoutRepository.SetCheckoutDefaultShippingOption(newCheckout, user);

					await this._checkoutRepository.AddAsync(newCheckout);

					await this._checkoutRepository.UpdateCheckoutFromLastOrderAsync(newCheckout.Id, userId);

					checkout = newCheckout;
				}
			}

			return checkout;
		}

		public async Task<CheckoutViewModel?> GetUserCheckout(Checkout? checkout)
		{
			CheckoutViewModel? model = null;

			if (checkout != null)
			{
				ApplicationUser? user = checkout.User;

				if (!string.IsNullOrWhiteSpace(checkout.UserId) && (user != null))
				{
					MemberAddressViewModel addressViewModel = new MemberAddressViewModel();
					if (user.Addresses.Any())
					{
						addressViewModel.SavedAddresses = user.Addresses
							.Select(a => new MemberAddressItemViewModel
							{
								Id = a.Id,
								Street = a.Street,
								City = a.City,
								ZipCode = a.ZipCode,
								Country = a.Country,
								PhoneNumber = a.PhoneNumber,
								IsShippingAddress = a.IsShippingAddress
							}).ToList();
					}

					if (checkout.ShippingAddressId.HasValue)
					{
						addressViewModel.SelectedShippingAddressId = checkout.ShippingAddressId;
					}

					if (checkout.BillingAddressId.HasValue)
					{
						addressViewModel.SelectedBillingAddressId = checkout.BillingAddressId;
					}

					ShippingOptionsViewModel shippingModel = CreateShippingOptions(user);

					PaymentMethodViewModel paymentModel = new PaymentMethodViewModel{
						SelectedPaymentOption = checkout.PaymentMethod.Code!.Value
					};

					if (checkout.PaymentDetailsId != null && checkout.PaymentDetails != null)
					{
						PaymentDetails lastPaymentDetails = checkout.PaymentDetails;

						string checkoutCardNumber = lastPaymentDetails.CardNumber;

						string maskedCardNumber = checkoutCardNumber.Length > 4
							? new string('*', checkoutCardNumber.Length - 4) + checkoutCardNumber.Substring(checkoutCardNumber.Length - 4)
							: checkoutCardNumber;

						paymentModel.CreditCardDetails = new CreditCardFormViewModel()
						{
							CardNumber = maskedCardNumber,
							NameOnCard = lastPaymentDetails.NameOnCard,
							ExpMonth = lastPaymentDetails.ExpMonth,
							ExpYear = lastPaymentDetails.ExpYear,
						};
					}


					decimal deliveryCost = await this._checkoutRepository
							.GetCheckoutDeliveryCostAsync(checkout.UserId, checkout.SubTotal);

					OrderSummaryViewModel orderSummary = new OrderSummaryViewModel
					{
						Products = checkout.ShoppingCart
											.ShoppingCartItems
											.Select(item => new OrderProductViewModel()
											{
												ProductId = item.ProductId,
												Name = item.Product.Name,
												UnitPrice = item.Price,
												Quantity = item.Quantity,
												ImageUrl = item.Product.ImageUrl,
												Size = item.ProductSize
											})
											.ToList(),
						DeliveryCost = deliveryCost,
						Subtotal = checkout.SubTotal
					};

					model = new CheckoutViewModel
					{
						IsGuest = false,
						UserId = checkout.UserId,
						MemberAddress = addressViewModel,
						Shipping = shippingModel,
						Payment = paymentModel,
						Summary = orderSummary
					};
				}
				else
				{
					GuestAddressViewModel addressViewModel = new GuestAddressViewModel
					{
						FullName = checkout.GuestName!,
						Email = checkout.GuestEmail!
					};

					if ((checkout.ShippingAddressId != null) &&
						(checkout.ShippingAddress != null))
					{
						addressViewModel.ShippingAddress = new GuestShippingAddressViewModel
						{
							Street = checkout.ShippingAddress.Street,
							City = checkout.ShippingAddress.City,
							ZipCode = checkout.ShippingAddress.ZipCode,
							Country = checkout.ShippingAddress.Country,
							PhoneNumber = checkout.ShippingAddress.PhoneNumber
						};
					}

					if ((checkout.BillingAddressId != null) && 
						(checkout.BillingAddress != null))
					{
						addressViewModel.BillingAddress = new GuestBillingAddressViewModel
						{
							BillingStreet = checkout.BillingAddress.Street,
							BillingCity = checkout.BillingAddress.City,
							BillingZipCode = checkout.BillingAddress.ZipCode,
							BillingCountry = checkout.BillingAddress.Country,
							BillingPhoneNumber = checkout.BillingAddress.PhoneNumber
						};
					}

					ShippingOptionsViewModel shippingModel = CreateShippingOptions(user);

					CreditCardFormViewModel? creditCardDetails = null;
					if (checkout.PaymentDetailsId != null && checkout.PaymentDetails != null)
					{
						PaymentDetails lastPaymentDetails = checkout.PaymentDetails;
						string checkoutCardNumber = lastPaymentDetails.CardNumber;

						string maskedCardNumber = checkoutCardNumber.Length > 4
							? new string('*', checkoutCardNumber.Length - 4) + checkoutCardNumber.Substring(checkoutCardNumber.Length - 4)
							: checkoutCardNumber;

						creditCardDetails = new CreditCardFormViewModel
						{
							CardNumber = maskedCardNumber,
							NameOnCard = lastPaymentDetails.NameOnCard,
							ExpMonth = lastPaymentDetails.ExpMonth,
							ExpYear = lastPaymentDetails.ExpYear
						};
					}

					PaymentMethodViewModel paymentModel = new PaymentMethodViewModel
					{
						SelectedPaymentOption = checkout.PaymentMethod.Code!.Value,
						CreditCardDetails = creditCardDetails
					};

					decimal deliveryCost = await this._checkoutRepository
								.GetCheckoutDeliveryCostAsync(checkout.GuestId, checkout.SubTotal);

					OrderSummaryViewModel orderSummary = new OrderSummaryViewModel
					{
						Products = checkout.ShoppingCart
											.ShoppingCartItems
											.Select(item => new OrderProductViewModel()
											{
												ProductId = item.ProductId,
												Name = item.Product.Name,
												UnitPrice = item.Price,
												Quantity = item.Quantity,
												ImageUrl = item.Product.ImageUrl,
												Size = item.ProductSize
											})
											.ToList(),
						DeliveryCost = deliveryCost,
						Subtotal = checkout.SubTotal
					};

					model = new CheckoutViewModel
					{
						IsGuest = true,
						GuestId = checkout.GuestId,
						GuestAddress = addressViewModel,
						Shipping = shippingModel,
						Payment = paymentModel,
						Summary = orderSummary
					};
				}
			}

			return model;
		}

		public async Task<Checkout?> UpdateGuestCheckoutAsync(CheckoutViewModel? model)
		{
			Checkout? checkout = null;

			if (model != null)
			{
				if (model.IsGuest && model.GuestId != null)
				{
					Checkout? existingCheckout = await this._checkoutRepository
						.SingleOrDefaultAsync(c => c.GuestId == model.GuestId && 
											       c.Order == null &&
												   c.CompletedAt == null);

					if (existingCheckout != null)
					{
						if (model.GuestAddress != null && 
								!string.IsNullOrWhiteSpace(model.GuestAddress.FullName) &&
									!string.IsNullOrWhiteSpace(model.GuestAddress.Email))
						{

							//Check guest name
							if (string.IsNullOrWhiteSpace(existingCheckout.GuestName))
							{
								existingCheckout.GuestName = model.GuestAddress.FullName;
							}
							else
							{
								string existingGuestName = existingCheckout.GuestName.ToString().Trim();
								string modelGuestName = model.GuestAddress.FullName.ToString().Trim();

								if (existingGuestName != modelGuestName)
								{
									existingCheckout.GuestName = model.GuestAddress.FullName;
								}
							}

							//Check guest email
							if (string.IsNullOrWhiteSpace(existingCheckout.GuestEmail))
							{
								existingCheckout.GuestEmail = model.GuestAddress.Email;
							}
							else
							{
								string existingGuestEmail = existingCheckout.GuestEmail.ToString().Trim();
								string modelGuestEmail = model.GuestAddress.Email.ToString().Trim();

								if (existingGuestEmail != modelGuestEmail)
								{
									existingCheckout.GuestEmail = model.GuestAddress.Email;
								}
							}

							GuestShippingAddressViewModel shippingAddress = model.GuestAddress.ShippingAddress;

							if (string.IsNullOrWhiteSpace(shippingAddress.PhoneNumber) ||
								string.IsNullOrWhiteSpace(shippingAddress.Street) ||
								string.IsNullOrWhiteSpace(shippingAddress.Country) ||
								string.IsNullOrWhiteSpace(shippingAddress.City) ||
								string.IsNullOrWhiteSpace(shippingAddress.ZipCode))
								throw new InvalidOperationException("Invalid shipping address!");

							if (existingCheckout.ShippingAddress != null)
							{
								Address existingShippingAddress = existingCheckout.ShippingAddress;

								if (!AreGuestShippingAddressesSame(existingShippingAddress, shippingAddress))
								{
									Address newGuestShippingAddress = new Address
									{
										Street = shippingAddress.Street,
										City = shippingAddress.City,
										ZipCode = shippingAddress.ZipCode,
										Country = shippingAddress.Country,
										PhoneNumber = shippingAddress.PhoneNumber,
										GuestId = model.GuestId,
										IsShippingAddress = true,
										IsBillingAddress = false
									};

									existingCheckout.ShippingAddressId = newGuestShippingAddress.Id;
									existingCheckout.ShippingAddress = newGuestShippingAddress;

									await this._addressRepository.AddAsync(newGuestShippingAddress);
								}
							}
							else
							{
								Address newGuestShippingAddress = new Address
								{
									Street = shippingAddress.Street,
									City = shippingAddress.City,
									ZipCode = shippingAddress.ZipCode,
									Country = shippingAddress.Country,
									PhoneNumber = shippingAddress.PhoneNumber,
									GuestId = model.GuestId,
									IsShippingAddress = true,
									IsBillingAddress = false
								};

								existingCheckout.ShippingAddressId = newGuestShippingAddress.Id;
								existingCheckout.ShippingAddress = newGuestShippingAddress;

								await this._addressRepository.AddAsync(newGuestShippingAddress);
							}

							if (model.GuestAddress.BillingAddress != null)
							{
								GuestBillingAddressViewModel billingAddress = model.GuestAddress.BillingAddress;

								if (string.IsNullOrWhiteSpace(billingAddress.BillingPhoneNumber) ||
									string.IsNullOrWhiteSpace(billingAddress.BillingStreet) ||
									string.IsNullOrWhiteSpace(billingAddress.BillingCountry) ||
									string.IsNullOrWhiteSpace(billingAddress.BillingCity) ||
									string.IsNullOrWhiteSpace(billingAddress.BillingZipCode))
									throw new InvalidOperationException("Invalid billing address!");

								if (existingCheckout.BillingAddress != null)
								{
									Address existingBillingAddress = existingCheckout.BillingAddress;

									if (!AreGuestBillingAddressesSame(existingBillingAddress, billingAddress))
									{
										Address newGuestBillingAddress = new Address
										{
											Street = billingAddress.BillingStreet,
											City = billingAddress.BillingCity,
											ZipCode = billingAddress.BillingZipCode,
											Country = billingAddress.BillingCountry,
											PhoneNumber = billingAddress.BillingPhoneNumber,
											GuestId = model.GuestId,
											IsShippingAddress = false,
											IsBillingAddress = true
										};

										existingCheckout.BillingAddressId = newGuestBillingAddress.Id;
										existingCheckout.BillingAddress = newGuestBillingAddress;

										await this._addressRepository.AddAsync(newGuestBillingAddress);
									}
								}
								else
								{
									Address newGuestBillingAddress = new Address
									{
										Street = billingAddress.BillingStreet,
										City = billingAddress.BillingCity,
										ZipCode = billingAddress.BillingZipCode,
										Country = billingAddress.BillingCountry,
										PhoneNumber = billingAddress.BillingPhoneNumber,
										GuestId = model.GuestId,
										IsShippingAddress = false,
										IsBillingAddress = true
									};

									existingCheckout.BillingAddressId = newGuestBillingAddress.Id;
									existingCheckout.BillingAddress = newGuestBillingAddress;

									await this._addressRepository.AddAsync(newGuestBillingAddress);
								}
							}
							else
							{
								if (existingCheckout.BillingAddress != null)
								{
									existingCheckout.BillingAddress = null;

									await this._checkoutRepository.UpdateAsync(existingCheckout);
								}
							}
						}
						else
						{
							throw new InvalidOperationException("Guest address is required!");
						}

						if (model.Shipping != null)
						{
							ShippingOptionItemViewModel selectedShippingOption = model.Shipping.SelectedShippingOption;

							if (!AreShippingOptionsSame(existingCheckout, selectedShippingOption))
							{
								existingCheckout.ShippingOption = selectedShippingOption.Name;
								existingCheckout.EstimatedDeliveryStart = selectedShippingOption.EstimatedDeliveryStart;
								existingCheckout.EstimatedDeliveryEnd = selectedShippingOption.EstimatedDeliveryEnd;
								existingCheckout.ShippingPrice = selectedShippingOption.Price;

								await this._checkoutRepository.UpdateAsync(existingCheckout);
							}
						}

						if (model.Payment != null)
						{
							PaymentMethodCode modelPaymentMethodCode = model.Payment.SelectedPaymentOption;

							PaymentMethod? paymentMethod = await this._paymentMethodRepository
								.FirstOrDefaultAsync(pm => pm.Code == modelPaymentMethodCode);

							if (paymentMethod == null)
								throw new InvalidOperationException("Payment method not found!");

							if (model.Payment.CreditCardDetails != null && paymentMethod.Code == PaymentMethodCode.CreditCard)
							{	
								CreditCardFormViewModel paymentDetails = model.Payment.CreditCardDetails;

								bool isExpMonthValid = paymentDetails.ExpMonth >= ExpMonthMin &&
													   paymentDetails.ExpMonth <= ExpMonthMax;

								bool isExpYearValid = paymentDetails.ExpYear >= ExpYearMin && 
													  paymentDetails.ExpYear <= ExpYearMax;

								int expMonth = paymentDetails.ExpMonth;
								int expYear = paymentDetails.ExpYear;

								if (expYear is >= 0 and < 100)
									expYear += 2000;

								var now = DateTime.UtcNow;
								var current = new DateTime(now.Year, now.Month, 1);
								var expiration = new DateTime(expYear, expMonth, 1);

								bool isExpirationValid = expiration >= current;

								if (string.IsNullOrWhiteSpace(paymentDetails.CardNumber) ||
									string.IsNullOrWhiteSpace(paymentDetails.NameOnCard) ||
									(!isExpMonthValid) || (!isExpYearValid) || (!isExpirationValid))
									throw new InvalidOperationException("Invalid payment details!");

								if (existingCheckout.PaymentMethod.Code != paymentMethod.Code &&
									existingCheckout.PaymentMethod.Name != paymentMethod.Name)
								{
									existingCheckout.PaymentMethod = paymentMethod;
									existingCheckout.PaymentMethodId = paymentMethod.Id;

									PaymentDetails newPaymentDetails = new PaymentDetails
									{
										CardNumber = paymentDetails.CardNumber,
										NameOnCard = paymentDetails.NameOnCard,
										ExpMonth = paymentDetails.ExpMonth,
										ExpYear = paymentDetails.ExpYear,
										Status = DefaultStartingPaymentStatus,
										Checkout = existingCheckout
									};

									existingCheckout.PaymentDetailsId = newPaymentDetails.Id;
									existingCheckout.PaymentDetails = newPaymentDetails;

									await this._paymentDetailsRepository.AddAsync(newPaymentDetails);
								}
								else
								{
									PaymentDetails existingPaymentDetails = existingCheckout.PaymentDetails!;

									if (existingPaymentDetails != null)
									{
										bool arePaymentDetailsSame = ArePaymentDetailsSame(existingPaymentDetails, paymentDetails);
										if (!arePaymentDetailsSame)
										{
											PaymentDetails newPaymentDetails = new PaymentDetails
											{
												CardNumber = paymentDetails.CardNumber,
												NameOnCard = paymentDetails.NameOnCard,
												ExpMonth = paymentDetails.ExpMonth,
												ExpYear = paymentDetails.ExpYear,
												Status = DefaultStartingPaymentStatus,
												Checkout = existingCheckout
											};

											existingCheckout.PaymentDetailsId = newPaymentDetails.Id;
											existingCheckout.PaymentDetails = newPaymentDetails;

											await this._paymentDetailsRepository.AddAsync(newPaymentDetails);
										}
									}
									else
									{
										PaymentDetails newPaymentDetails = new PaymentDetails
										{
											CardNumber = paymentDetails.CardNumber,
											NameOnCard = paymentDetails.NameOnCard,
											ExpMonth = paymentDetails.ExpMonth,
											ExpYear = paymentDetails.ExpYear,
											Status = DefaultStartingPaymentStatus,
											Checkout = existingCheckout
										};

										existingCheckout.PaymentDetailsId = newPaymentDetails.Id;
										existingCheckout.PaymentDetails = newPaymentDetails;

										await this._paymentDetailsRepository.AddAsync(newPaymentDetails);
									}
								}

							}
							else if (paymentMethod.Code != PaymentMethodCode.CreditCard)
							{
								if (existingCheckout.PaymentMethod.Code != paymentMethod.Code &&
									existingCheckout.PaymentMethod.Name != paymentMethod.Name)
								{
									if (existingCheckout.PaymentDetails != null)
									{
										existingCheckout.PaymentDetailsId = null;
										existingCheckout.PaymentDetails = null;
									}

									existingCheckout.PaymentMethod = paymentMethod;
									existingCheckout.PaymentMethodId = paymentMethod.Id;

									await this._checkoutRepository.UpdateAsync(existingCheckout);
								}
							}
						}
					}
					else
					{
						throw new InvalidOperationException("Checkout not found for the guest!");
					}

					checkout = existingCheckout;
				}
			}

			return checkout;
		}

		public async Task<Checkout?> UpdateUserCheckoutAsync(CheckoutViewModel? model)
		{
			Checkout? checkout = null;

			if (model != null)
			{
				if (!model.IsGuest && model.UserId != null)
				{
					Checkout? existingCheckout = await this._checkoutRepository
								.SingleOrDefaultAsync(c => c.UserId == model.UserId && 
														   c.Order == null && 
														   c.CompletedAt == null);

					ApplicationUser? user = await this._userManager
								.FindByIdAsync(model.UserId);

					if (existingCheckout != null && user != null)
					{
						if (model.MemberAddress != null)
						{
							if (model.MemberAddress.SelectedShippingAddressId.HasValue)
							{
								int selectedShippingAddressId = model.MemberAddress.SelectedShippingAddressId.Value;

								Address? address = await this._addressRepository
									.GetByIdAsync(selectedShippingAddressId);

								bool isAddressExistIsUserAddresses = user.Addresses
											.Where(a => a.IsShippingAddress)
											.Any(a => a.Id == selectedShippingAddressId);

								if (address == null || !isAddressExistIsUserAddresses)
								{
									throw new InvalidOperationException("Shipping address not found!");
								}

								if (existingCheckout.ShippingAddressId != address.Id)
								{
									existingCheckout.ShippingAddressId = address.Id;
									existingCheckout.ShippingAddress = address;

									await this._checkoutRepository.UpdateAsync(existingCheckout);
								}
							}
							else if (model.MemberAddress.NewShippingAddress != null)
							{
								MemberAddressItemViewModel newAddress = model.MemberAddress.NewShippingAddress;

								if (string.IsNullOrWhiteSpace(newAddress.PhoneNumber) ||
									string.IsNullOrWhiteSpace(newAddress.Street) ||
									string.IsNullOrWhiteSpace(newAddress.Country) ||
									string.IsNullOrWhiteSpace(newAddress.City) ||
									string.IsNullOrWhiteSpace(newAddress.ZipCode))
									throw new InvalidOperationException("Invalid shipping address!");

								if (existingCheckout.ShippingAddressId != null && 
									existingCheckout.ShippingAddress != null)
								{
									Address existingShippingAddress = existingCheckout.ShippingAddress;

									if (!AreMemberShippingAddressesSame(existingShippingAddress, newAddress))
									{
										Address newMemberShippingAddress = new Address
										{
											Street = newAddress.Street,
											City = newAddress.City,
											ZipCode = newAddress.ZipCode,
											Country = newAddress.Country,
											PhoneNumber = newAddress.PhoneNumber,
											UserId = user.Id,
											IsShippingAddress = true,
											IsBillingAddress = false
										};

										existingCheckout.ShippingAddressId = newMemberShippingAddress.Id;
										existingCheckout.ShippingAddress = newMemberShippingAddress;

										user.Addresses.Add(newMemberShippingAddress);

										await this._addressRepository.AddAsync(newMemberShippingAddress);
									}
								}
								else
								{
									Address newMemberShippingAddress = new Address
									{
										Street = newAddress.Street,
										City = newAddress.City,
										ZipCode = newAddress.ZipCode,
										Country = newAddress.Country,
										PhoneNumber = newAddress.PhoneNumber,
										UserId = user.Id,
										IsShippingAddress = true,
										IsBillingAddress = false
									};

									existingCheckout.ShippingAddressId = newMemberShippingAddress.Id;
									existingCheckout.ShippingAddress = newMemberShippingAddress;

									user.Addresses.Add(newMemberShippingAddress);

									await this._addressRepository.AddAsync(newMemberShippingAddress);
								}

								bool isNewAddressUserDefault = newAddress.DefaultShipping == true;
								if (isNewAddressUserDefault)
								{
									user.DefaultShippingAddressId = existingCheckout.ShippingAddressId;
									user.DefaultShippingAddress = existingCheckout.ShippingAddress;

									await this._userManager.UpdateAsync(user);
								}
							}
							else
							{
								throw new InvalidOperationException("ShippingAddressIsRequired");
							}

							if (model.MemberAddress.SelectedBillingAddressId.HasValue)
							{
								int selectedBillingAddressId = model.MemberAddress.SelectedBillingAddressId.Value;

								Address? address = await this._addressRepository
									.GetByIdAsync(selectedBillingAddressId);

								bool isAddressExistIsUserAddresses = user.Addresses
											.Where(a => a.IsBillingAddress)
											.Any(a => a.Id == selectedBillingAddressId);

								if (address == null || !isAddressExistIsUserAddresses)
								{
									throw new InvalidOperationException("Billing address not found!");
								}

								if (existingCheckout.BillingAddressId != address.Id)
								{
									existingCheckout.BillingAddressId = address.Id;
									existingCheckout.BillingAddress = address;

									await this._checkoutRepository.UpdateAsync(existingCheckout);
								}
							}
							else if (model.MemberAddress.NewBillingAddress != null)
							{
								MemberAddressItemViewModel newAddress = model.MemberAddress.NewBillingAddress;

								if (string.IsNullOrWhiteSpace(newAddress.PhoneNumber) ||
									string.IsNullOrWhiteSpace(newAddress.Street) ||
									string.IsNullOrWhiteSpace(newAddress.Country) ||
									string.IsNullOrWhiteSpace(newAddress.City) ||
									string.IsNullOrWhiteSpace(newAddress.ZipCode))
									throw new InvalidOperationException("Invalid billing address!");

								if (existingCheckout.BillingAddressId != null &&
									existingCheckout.BillingAddress != null)
								{
									Address existingBillingAddress = existingCheckout.BillingAddress;

									if (!AreMemberShippingAddressesSame(existingBillingAddress, newAddress))
									{
										Address newMemberBillingAddress = new Address
										{
											Street = newAddress.Street,
											City = newAddress.City,
											ZipCode = newAddress.ZipCode,
											Country = newAddress.Country,
											PhoneNumber = newAddress.PhoneNumber,
											UserId = user.Id,
											IsShippingAddress = false,
											IsBillingAddress = true
										};

										existingCheckout.BillingAddressId = newMemberBillingAddress.Id;
										existingCheckout.BillingAddress = newMemberBillingAddress;

										user.Addresses.Add(newMemberBillingAddress);

										await this._addressRepository.AddAsync(newMemberBillingAddress);
									}
								}
								else
								{
									Address newMemberBillingAddress = new Address
									{
										Street = newAddress.Street,
										City = newAddress.City,
										ZipCode = newAddress.ZipCode,
										Country = newAddress.Country,
										PhoneNumber = newAddress.PhoneNumber,
										UserId = user.Id,
										IsShippingAddress = false,
										IsBillingAddress = true
									};

									existingCheckout.BillingAddressId = newMemberBillingAddress.Id;
									existingCheckout.BillingAddress = newMemberBillingAddress;

									user.Addresses.Add(newMemberBillingAddress);

									await this._addressRepository.AddAsync(newMemberBillingAddress);
								}

								bool isNewAddressUserDefault = newAddress.DefaultBilling == true;
								if (isNewAddressUserDefault)
								{
									user.DefaultBillingAddressId = existingCheckout.BillingAddressId;
									user.DefaultBillingAddress = existingCheckout.BillingAddress;

									await this._userManager.UpdateAsync(user);
								}
							}
							else
							{
								if (existingCheckout.BillingAddress != null)
								{
									existingCheckout.BillingAddress = null;

									await this._checkoutRepository.UpdateAsync(existingCheckout);
								}
							}
						}
						else
						{
							throw new InvalidOperationException("Member address is required!");
						}

						if (model.Shipping != null)
						{
							ShippingOptionItemViewModel selectedShippingOption = model.Shipping.SelectedShippingOption;

							if (!AreShippingOptionsSame(existingCheckout, selectedShippingOption))
							{
								existingCheckout.ShippingOption = selectedShippingOption.Name;
								existingCheckout.EstimatedDeliveryStart = selectedShippingOption.EstimatedDeliveryStart;
								existingCheckout.EstimatedDeliveryEnd = selectedShippingOption.EstimatedDeliveryEnd;
								existingCheckout.ShippingPrice = selectedShippingOption.Price;

								await this._checkoutRepository.UpdateAsync(existingCheckout);
							}
						}

						if (model.Payment != null)
						{
							PaymentMethodCode modelPaymentMethodCode = model.Payment.SelectedPaymentOption;

							PaymentMethod? paymentMethod = await this._paymentMethodRepository
								.FirstOrDefaultAsync(pm => pm.Code == modelPaymentMethodCode);

							if (paymentMethod == null)
								throw new InvalidOperationException("Payment method not found!");

							if (model.Payment.CreditCardDetails != null && paymentMethod.Code == PaymentMethodCode.CreditCard)
							{
								CreditCardFormViewModel paymentDetails = model.Payment.CreditCardDetails;

								bool isExpMonthValid = paymentDetails.ExpMonth >= ExpMonthMin &&
													   paymentDetails.ExpMonth <= ExpMonthMax;

								bool isExpYearValid = paymentDetails.ExpYear >= ExpYearMin &&
													  paymentDetails.ExpYear <= ExpYearMax;

								int expMonth = paymentDetails.ExpMonth;
								int expYear = paymentDetails.ExpYear;

								if (expYear is >= 0 and < 100)
									expYear += 2000;

								var now = DateTime.UtcNow;
								var current = new DateTime(now.Year, now.Month, 1);
								var expiration = new DateTime(expYear, expMonth, 1);

								bool isExpirationValid = expiration >= current;

								if (string.IsNullOrWhiteSpace(paymentDetails.CardNumber) ||
									string.IsNullOrWhiteSpace(paymentDetails.NameOnCard) ||
									(!isExpMonthValid) || (!isExpYearValid) || (!isExpirationValid))
									throw new InvalidOperationException("Invalid payment details!");

								if (existingCheckout.PaymentMethod.Code != paymentMethod.Code &&
									existingCheckout.PaymentMethod.Name != paymentMethod.Name)
								{
									existingCheckout.PaymentMethod = paymentMethod;
									existingCheckout.PaymentMethodId = paymentMethod.Id;

									
									PaymentDetails newPaymentDetails = new PaymentDetails
									{
										CardNumber = paymentDetails.CardNumber,
										NameOnCard = paymentDetails.NameOnCard,
										ExpMonth = paymentDetails.ExpMonth,
										ExpYear = paymentDetails.ExpYear,
										Status = DefaultStartingPaymentStatus,
										Checkout = existingCheckout
									};

									existingCheckout.PaymentDetailsId = newPaymentDetails.Id;
									existingCheckout.PaymentDetails = newPaymentDetails;

									await this._paymentDetailsRepository.AddAsync(newPaymentDetails);

								}
								else
								{
									PaymentDetails existingPaymentDetails = existingCheckout.PaymentDetails!;

									if (existingPaymentDetails != null)
									{
										bool arePaymentDetailsSame = ArePaymentDetailsSame(existingPaymentDetails, paymentDetails);
										if (!arePaymentDetailsSame)
										{
											PaymentDetails newPaymentDetails = new PaymentDetails
											{
												CardNumber = paymentDetails.CardNumber,
												NameOnCard = paymentDetails.NameOnCard,
												ExpMonth = paymentDetails.ExpMonth,
												ExpYear = paymentDetails.ExpYear,
												Status = DefaultStartingPaymentStatus,
												Checkout = existingCheckout
											};

											existingCheckout.PaymentDetailsId = newPaymentDetails.Id;
											existingCheckout.PaymentDetails = newPaymentDetails;

											await this._paymentDetailsRepository.AddAsync(newPaymentDetails);
										}
									}
									else
									{
										PaymentDetails newPaymentDetails = new PaymentDetails
										{
											CardNumber = paymentDetails.CardNumber,
											NameOnCard = paymentDetails.NameOnCard,
											ExpMonth = paymentDetails.ExpMonth,
											ExpYear = paymentDetails.ExpYear,
											Status = DefaultStartingPaymentStatus,
											Checkout = existingCheckout
										};

										existingCheckout.PaymentDetailsId = newPaymentDetails.Id;
										existingCheckout.PaymentDetails = newPaymentDetails;

										await this._paymentDetailsRepository.AddAsync(newPaymentDetails);
									}
								}

							}
							else if (paymentMethod.Code != PaymentMethodCode.CreditCard)
							{
								if (existingCheckout.PaymentMethod.Code != paymentMethod.Code &&
									existingCheckout.PaymentMethod.Name != paymentMethod.Name)
								{
									if (existingCheckout.PaymentDetails != null)
									{
										existingCheckout.PaymentDetailsId = null;
										existingCheckout.PaymentDetails = null;
									}

									existingCheckout.PaymentMethod = paymentMethod;
									existingCheckout.PaymentMethodId = paymentMethod.Id;

									await this._checkoutRepository.UpdateAsync(existingCheckout);
								}
							}

							bool isPaymentMethodUserDefault = model.Payment.DefaultPaymentMethod == true;
							bool isPaymentDetailsUserDefault = model.Payment.CreditCardDetails?.DefaultPaymentDetails == true;

							if (isPaymentMethodUserDefault)
							{
								user.DefaultPaymentMethodId = existingCheckout.PaymentMethodId;
								user.DefaultPaymentMethod = existingCheckout.PaymentMethod;
							}
							else if (isPaymentDetailsUserDefault)
							{
								user.DefaultPaymentDetailsId = existingCheckout.PaymentDetailsId;
								user.DefaultPaymentDetails = existingCheckout.PaymentDetails;
							}

							if (isPaymentMethodUserDefault || isPaymentDetailsUserDefault)
								await this._userManager.UpdateAsync(user);
						}
					}
					else
					{
						throw new InvalidOperationException("Checkout or User not found");
					}

					checkout = existingCheckout;
				}
			}

			return checkout;
		}


		private static ShippingOptionsViewModel CreateShippingOptions(ApplicationUser? user)
		{
			var today = DateTime.Today;

			var standardStart = AddBusinessDays(today, StandartShippingOptionDaysMin);
			var standardEnd = AddBusinessDays(today, StandartShippingOptionDaysMax);

			var expressStart = AddBusinessDays(today, ExpressShippingOptionDaysMin);
			var expressEnd = AddBusinessDays(today, ExpressShippingOptionDaysMax);

			var isMember = user != null;

			var options = new List<ShippingOptionItemViewModel>
			{
				new ShippingOptionItemViewModel
				{
					Name = StandartShippingOptionName,
					Description = isMember ? "Free shipping for members" : "Delivered via standard carrier",
					DateRange = $"{standardStart:MMM dd} – {standardEnd:MMM dd}",
					EstimatedDeliveryStart = standardStart,
					EstimatedDeliveryEnd = standardEnd,
					Price = isMember ? StandartShippingPriceForMembers : StandartShippingPriceForGuests
				},
				new ShippingOptionItemViewModel
				{
					Name = ExpressShippingOptionName,
					Description = "Fastest option via premium courier",
					DateRange = $"{expressStart:MMM dd} – {expressEnd:MMM dd}",
					EstimatedDeliveryStart = expressStart,
					EstimatedDeliveryEnd = expressEnd,
					Price = ExpressShippingPrice
				}
			};

			return new ShippingOptionsViewModel
			{
				SelectedShippingOption = options.First(),
				Options = options
			};
		}

		private static DateTime AddBusinessDays(DateTime startDate, int businessDays)
		{
			var current = startDate;
			while (businessDays > 0)
			{
				current = current.AddDays(1);
				if (current.DayOfWeek != DayOfWeek.Saturday && current.DayOfWeek != DayOfWeek.Sunday)
				{
					businessDays--;
				}
			}
			return current;
		}



		private static bool AreGuestShippingAddressesSame(Address checkoutAddress, GuestShippingAddressViewModel modelAddress)
		{
			bool areSame = true;

			if (checkoutAddress.Street != modelAddress.Street)
				areSame = false;

			if (checkoutAddress.City != modelAddress.City)
				areSame = false;

			if (checkoutAddress.ZipCode != modelAddress.ZipCode)
				areSame = false;

			if (checkoutAddress.Country != modelAddress.Country)
				areSame = false;

			if (checkoutAddress.PhoneNumber != modelAddress.PhoneNumber)
				areSame = false;

			return areSame;
		}

		private static bool AreMemberShippingAddressesSame(Address checkoutAddress, MemberAddressItemViewModel modelAddress)
		{
			bool areSame = true;

			if (checkoutAddress.Street != modelAddress.Street)
				areSame = false;
			if (checkoutAddress.City != modelAddress.City)
				areSame = false;
			if (checkoutAddress.ZipCode != modelAddress.ZipCode)
				areSame = false;
			if (checkoutAddress.Country != modelAddress.Country)
				areSame = false;
			if (checkoutAddress.PhoneNumber != modelAddress.PhoneNumber)
				areSame = false;

			return areSame;
		}

		private static bool AreGuestBillingAddressesSame(Address checkoutAddress, GuestBillingAddressViewModel modelAddress)
		{
			bool areSame = true;

			if (checkoutAddress.Street != modelAddress.BillingStreet)
				areSame = false;

			if (checkoutAddress.City != modelAddress.BillingCity)
				areSame = false;

			if (checkoutAddress.ZipCode != modelAddress.BillingZipCode)
				areSame = false;

			if (checkoutAddress.Country != modelAddress.BillingCountry)
				areSame = false;

			if (checkoutAddress.PhoneNumber != modelAddress.BillingPhoneNumber)
				areSame = false;

			return areSame;
		}

		private static bool ArePaymentDetailsSame(PaymentDetails checkoutDetails, CreditCardFormViewModel modelDetails)
		{
			bool areSame = true;

			string last4 = checkoutDetails.CardNumber.Length >= 4
				? checkoutDetails.CardNumber.Substring(checkoutDetails.CardNumber.Length - 4)
				: checkoutDetails.CardNumber;

			string maskedLast4 = modelDetails.CardNumber.Length >= 4
				? modelDetails.CardNumber.Substring(modelDetails.CardNumber.Length - 4)
				: modelDetails.CardNumber;

			if (last4 != maskedLast4)
				areSame = false;
			else if (checkoutDetails.NameOnCard != modelDetails.NameOnCard)
				areSame = false;
			else if (checkoutDetails.ExpMonth != modelDetails.ExpMonth)
				areSame = false;
			else if (checkoutDetails.ExpYear != modelDetails.ExpYear)
				areSame = false;

			return areSame;
		}

		private static bool AreShippingOptionsSame(Checkout checkout, ShippingOptionItemViewModel modelOption)
		{
			bool areSame = true;

			if (checkout.ShippingOption != modelOption.Name)
				areSame = false;
			else if (checkout.EstimatedDeliveryStart != modelOption.EstimatedDeliveryStart)
				areSame = false;
			else if (checkout.EstimatedDeliveryEnd != modelOption.EstimatedDeliveryEnd)
				areSame = false;
			else if (checkout.ShippingPrice != modelOption.Price)
				areSame = false;

			return areSame;
		}

	}
}

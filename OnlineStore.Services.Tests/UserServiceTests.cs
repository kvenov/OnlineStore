using Microsoft.AspNetCore.Identity;
using MockQueryable;
using Moq;
using NUnit.Framework.Legacy;
using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Services.Core;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.User;

namespace OnlineStore.Services.Tests
{
	[TestFixture]
	public class UserServiceTests
	{
		private Mock<UserManager<ApplicationUser>> _mockUserManager;
		private Mock<IRepository<PaymentMethod, int>> _mockPaymentMethodRepo;
		private Mock<IRepository<PaymentDetails, int>> _mockPaymentDetailsRepo;
		private IUserService _userService;

		[SetUp]
		public void Setup()
		{
			var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
			_mockUserManager = new Mock<UserManager<ApplicationUser>>(
				userStoreMock.Object, null, null, null, null, null, null, null, null);

			_mockPaymentMethodRepo = new Mock<IRepository<PaymentMethod, int>>();
			_mockPaymentDetailsRepo = new Mock<IRepository<PaymentDetails, int>>();

			_userService = new UserService(
				_mockUserManager.Object,
				_mockPaymentMethodRepo.Object,
				_mockPaymentDetailsRepo.Object);
		}

		#region GetUserSettingsAsync Tests

		[Test]
		public async Task GetUserSettingsAsync_NullOrWhitespaceUserId_ReturnsNull()
		{
			var result = await _userService.GetUserSettingsAsync(null);
			Assert.That(result, Is.Null);

			result = await _userService.GetUserSettingsAsync("");
			Assert.That(result, Is.Null);

			result = await _userService.GetUserSettingsAsync("   ");
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetUserSettingsAsync_UserNotFound_ReturnsNull()
		{
			_mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
				.ReturnsAsync((ApplicationUser?)null);

			var result = await _userService.GetUserSettingsAsync("someId");
			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetUserSettingsAsync_UserFound_ReturnsAccountSettingsViewModelWithData()
		{
			var user = new ApplicationUser
			{
				Id = "userId",
				FullName = "John Doe",
				UserName = "johndoe",
				Email = "john@example.com",
				PhoneNumber = "1234567890",
				DefaultBillingAddressId = 1,
				DefaultShippingAddressId = 2,
				DefaultPaymentMethodId = 3,
				DefaultPaymentDetailsId = 4,
				Addresses = new List<Address>
				{
					new Address
					{
						Id = 2,
						Street = "123 Main St",
						City = "Cityville",
						ZipCode = "12345",
						Country = "Countryland",
						PhoneNumber = "1234567890"
					}
				}
			};

			_mockUserManager.Setup(m => m.FindByIdAsync("userId"))
				.ReturnsAsync(user);

			// Setup payment method repository
			var paymentMethods = new List<PaymentMethod>
			{
				new PaymentMethod { Id = 3, Name = "Visa", Code = OnlineStore.Data.Models.Enums.PaymentMethodCode.CreditCard },
				new PaymentMethod { Id = 5, Name = "PayPal", Code = OnlineStore.Data.Models.Enums.PaymentMethodCode.PayPal }
			};
			var pmQueryable = paymentMethods.BuildMock();
			_mockPaymentMethodRepo.Setup(r => r.GetAllAttached())
				.Returns(pmQueryable);

			// Setup payment details repository
			var paymentDetails = new List<PaymentDetails>
			{
				new PaymentDetails
				{
					Id = 4,
					CardNumber = "1234567812345678",
					NameOnCard = "John Doe",
					ExpMonth = 12,
					ExpYear = 2030
				}
			};
			var pdQueryable = paymentDetails.BuildMock();
			_mockPaymentDetailsRepo.Setup(r => r.GetAllAttached())
				.Returns(pdQueryable);

			var result = await _userService.GetUserSettingsAsync("userId");

			Assert.That(result, Is.Not.Null);

			Assert.That(user.FullName, Is.EqualTo(result.Profile.FullName));
			Assert.That(user.UserName, Is.EqualTo(result.Profile.Username));
			Assert.That(user.Email, Is.EqualTo(result.Profile.Email));
			Assert.That(user.PhoneNumber, Is.EqualTo(result.Profile.PhoneNumber));

			Assert.That(user.DefaultShippingAddressId, Is.EqualTo(result.Addresses.DefaultShippingAddressId));
			Assert.That(user.DefaultBillingAddressId, Is.EqualTo(result.Addresses.DefaultBillingAddressId));

			Assert.That(result.AvailableAddresses.Any(), Is.True);
			Assert.That(result.AvailablePaymentMethods.Any(), Is.True);
			Assert.That(result.AvailablePaymentDetails.Any(), Is.True);

			var firstAddress = result.AvailableAddresses.First();
			StringAssert.Contains("123 Main St", firstAddress.DisplayName);
			Assert.That(2, Is.EqualTo(firstAddress.Id));

			var firstPaymentMethod = result.AvailablePaymentMethods.First();
			Assert.That(3, Is.EqualTo(firstPaymentMethod.Id));
			Assert.That("Visa", Is.EqualTo(firstPaymentMethod.MethodName));

			var firstPaymentDetails = result.AvailablePaymentDetails.First();
			Assert.That(4, Is.EqualTo(firstPaymentDetails.Id));
			StringAssert.Contains("****5678", firstPaymentDetails.CardSummary);
		}

		#endregion

		#region ChangeUserProfileAsync Tests

		[Test]
		public async Task ChangeUserProfileAsync_NullModelOrInvalidUserId_ReturnsFalse()
		{
			var (result, msg) = await _userService.ChangeUserProfileAsync(null, null);
			Assert.That(result, Is.False);

			(result, msg) = await _userService.ChangeUserProfileAsync(null, "");
			Assert.That(result, Is.False);

			(result, msg) = await _userService.ChangeUserProfileAsync(new UpdateProfileInputModel(), null);
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task ChangeUserProfileAsync_UserNotFound_ReturnsFalse()
		{
			_mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
				.ReturnsAsync((ApplicationUser?)null);

			var model = new UpdateProfileInputModel
			{
				FullName = "New Name",
				Username = "newuser",
				Email = "newemail@example.com",
				PhoneNumber = "11111111"
			};

			var (result, msg) = await _userService.ChangeUserProfileAsync(model, "userId");
			Assert.That(result, Is.False);
			StringAssert.AreEqualIgnoringCase("Something went wrong!", msg);
		}

		[Test]
		public async Task ChangeUserProfileAsync_NoChanges_ReturnsFalseWithMessage()
		{
			var user = new ApplicationUser
			{
				FullName = "Same Name",
				UserName = "sameuser",
				Email = "same@example.com",
				PhoneNumber = "11111111"
			};

			_mockUserManager.Setup(m => m.FindByIdAsync("userId"))
				.ReturnsAsync(user);

			var model = new UpdateProfileInputModel
			{
				FullName = "same name", // case insensitive equals
				Username = "SAMEUSER",
				Email = "SAME@EXAMPLE.COM",
				PhoneNumber = "11111111"
			};

			var (result, msg) = await _userService.ChangeUserProfileAsync(model, "userId");

			Assert.That(result, Is.False);
			StringAssert.Contains("Nothing new to change", msg);

			// Ensure UpdateAsync NOT called
			_mockUserManager.Verify(m => m.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Never);
		}

		[Test]
		public async Task ChangeUserProfileAsync_ChangesDetected_UpdatesUser_ReturnsSuccess()
		{
			var user = new ApplicationUser
			{
				FullName = "Old Name",
				UserName = "olduser",
				Email = "old@example.com",
				PhoneNumber = "0000"
			};

			_mockUserManager.Setup(m => m.FindByIdAsync("userId"))
				.ReturnsAsync(user);

			_mockUserManager.Setup(m => m.UpdateAsync(user))
				.ReturnsAsync(IdentityResult.Success);

			var model = new UpdateProfileInputModel
			{
				FullName = "New Name",
				Username = "newuser",
				Email = "new@example.com",
				PhoneNumber = "1111"
			};

			var (result, msg) = await _userService.ChangeUserProfileAsync(model, "userId");

			Assert.That(result, Is.True);
			StringAssert.Contains("successfully updated", msg);

			_mockUserManager.Verify(m => m.UpdateAsync(It.Is<ApplicationUser>(u =>
				u.FullName == model.FullName &&
				u.UserName == model.Username &&
				u.Email == model.Email &&
				u.PhoneNumber == model.PhoneNumber)), Times.Once);
		}

		#endregion

		#region ChangeUserPasswordAsync Tests

		[Test]
		public async Task ChangeUserPasswordAsync_InvalidInput_ReturnsFalse()
		{
			// Null model
			var (result, msg) = await _userService.ChangeUserPasswordAsync(null, "userId");
			Assert.That(result, Is.False);

			// Null userId
			var model = new ChangePasswordInputModel
			{
				CurrentPassword = "OldPass1",
				NewPassword = "NewPass1A",
				ConfirmNewPassword = "NewPass1A"
			};
			(result, msg) = await _userService.ChangeUserPasswordAsync(model, null);
			Assert.That(result, Is.False);

			// Empty passwords
			model = new ChangePasswordInputModel
			{
				CurrentPassword = "",
				NewPassword = "",
				ConfirmNewPassword = ""
			};
			(result, msg) = await _userService.ChangeUserPasswordAsync(model, "userId");
			Assert.That(result, Is.False);

			// New password != Confirm password
			model = new ChangePasswordInputModel
			{
				CurrentPassword = "OldPass1",
				NewPassword = "NewPass1A",
				ConfirmNewPassword = "DiffPass"
			};
			(result, msg) = await _userService.ChangeUserPasswordAsync(model, "userId");
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task ChangeUserPasswordAsync_UserNotFound_ReturnsFalse()
		{
			_mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
				.ReturnsAsync((ApplicationUser?)null);

			var model = new ChangePasswordInputModel
			{
				CurrentPassword = "OldPass1",
				NewPassword = "NewPass1A",
				ConfirmNewPassword = "NewPass1A"
			};

			var (result, msg) = await _userService.ChangeUserPasswordAsync(model, "userId");

			Assert.That(result, Is.False);
		}

		[Test]
		public async Task ChangeUserPasswordAsync_NewPasswordSameAsOld_ReturnsFalse()
		{
			var user = new ApplicationUser();
			_mockUserManager.Setup(m => m.FindByIdAsync("userId"))
				.ReturnsAsync(user);

			var model = new ChangePasswordInputModel
			{
				CurrentPassword = "OldPass1",
				NewPassword = "OldPass1",
				ConfirmNewPassword = "OldPass1"
			};

			var (result, msg) = await _userService.ChangeUserPasswordAsync(model, "userId");

			Assert.That(result, Is.False);
			StringAssert.Contains("The passwords are idetified as same!", msg);
		}

		[Test]
		public async Task ChangeUserPasswordAsync_ChangePasswordFails_ReturnsFalse()
		{
			var user = new ApplicationUser();
			_mockUserManager.Setup(m => m.FindByIdAsync("userId"))
				.ReturnsAsync(user);

			_mockUserManager.Setup(m => m.ChangePasswordAsync(user, "OldPass1", "NewPass1A"))
				.ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error" }));

			var model = new ChangePasswordInputModel
			{
				CurrentPassword = "OldPass1",
				NewPassword = "NewPass1A",
				ConfirmNewPassword = "NewPass1A"
			};

			var (result, msg) = await _userService.ChangeUserPasswordAsync(model, "userId");

			Assert.That(result, Is.False);
			StringAssert.Contains("Something went wrong!", msg);
		}

		[Test]
		public async Task ChangeUserPasswordAsync_ChangePasswordSuccess_ReturnsTrue()
		{
			var user = new ApplicationUser();
			_mockUserManager.Setup(m => m.FindByIdAsync("userId"))
				.ReturnsAsync(user);

			_mockUserManager.Setup(m => m.ChangePasswordAsync(user, "OldPass1", "NewPass1A"))
				.ReturnsAsync(IdentityResult.Success);

			var model = new ChangePasswordInputModel
			{
				CurrentPassword = "OldPass1",
				NewPassword = "NewPass1A",
				ConfirmNewPassword = "NewPass1A"
			};

			var (result, msg) = await _userService.ChangeUserPasswordAsync(model, "userId");

			Assert.That(result, Is.True);
			StringAssert.Contains("successfully changed", msg);
		}

		#endregion

		#region ChangeUserAddressesAsync Tests

		[Test]
		public async Task ChangeUserAddressesAsync_NullModelOrInvalidUserId_ReturnsFalse()
		{
			var (result, msg) = await _userService.ChangeUserAddressesAsync(null, null);
			Assert.That(result, Is.False);

			(result, msg) = await _userService.ChangeUserAddressesAsync(null, "");
			Assert.That(result, Is.False);

			(result, msg) = await _userService.ChangeUserAddressesAsync(new UpdateAddressesInputModel(), null);
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task ChangeUserAddressesAsync_UserNotFound_ReturnsFalse()
		{
			_mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
				.ReturnsAsync((ApplicationUser?)null);

			var model = new UpdateAddressesInputModel
			{
				DefaultBillingAddressId = 1,
				DefaultShippingAddressId = 2
			};

			var (result, msg) = await _userService.ChangeUserAddressesAsync(model, "userId");
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task ChangeUserAddressesAsync_NoChanges_ReturnsFalseWithMessage()
		{
			var user = new ApplicationUser
			{
				DefaultBillingAddressId = 1,
				DefaultShippingAddressId = 2
			};

			_mockUserManager.Setup(m => m.FindByIdAsync("userId"))
				.ReturnsAsync(user);

			var model = new UpdateAddressesInputModel
			{
				DefaultBillingAddressId = 1,
				DefaultShippingAddressId = 2
			};

			var (result, msg) = await _userService.ChangeUserAddressesAsync(model, "userId");

			Assert.That(result, Is.False);
			StringAssert.Contains("The User has already these addresses as defaults!", msg);
		}

		[Test]
		public async Task ChangeUserAddressesAsync_ChangesDetected_UpdatesUser_ReturnsSuccess()
		{
			var user = new ApplicationUser
			{
				DefaultBillingAddressId = 1,
				DefaultShippingAddressId = 2
			};

			_mockUserManager.Setup(m => m.FindByIdAsync("userId"))
				.ReturnsAsync(user);

			_mockUserManager.Setup(m => m.UpdateAsync(user))
				.ReturnsAsync(IdentityResult.Success);

			var model = new UpdateAddressesInputModel
			{
				DefaultBillingAddressId = 3,
				DefaultShippingAddressId = 4
			};

			var (result, msg) = await _userService.ChangeUserAddressesAsync(model, "userId");

			Assert.That(result, Is.True);
			StringAssert.Contains("successfully updated", msg);

			_mockUserManager.Verify(m => m.UpdateAsync(It.Is<ApplicationUser>(u =>
				u.DefaultBillingAddressId == 3 &&
				u.DefaultShippingAddressId == 4)), Times.Once);
		}

		#endregion

		#region ChangeUserPaymentDataAsync Tests

		[Test]
		public async Task ChangeUserPaymentDataAsync_NullModelOrInvalidUserId_ReturnsFalse()
		{
			var (result, msg) = await _userService.ChangeUserPaymentDataAsync(null, null);
			Assert.That(result, Is.False);

			(result, msg) = await _userService.ChangeUserPaymentDataAsync(null, "");
			Assert.That(result, Is.False);

			(result, msg) = await _userService.ChangeUserPaymentDataAsync(new UpdatePaymentInputModel(), null);
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task ChangeUserPaymentDataAsync_UserNotFound_ReturnsFalse()
		{
			_mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
				.ReturnsAsync((ApplicationUser?)null);

			var model = new UpdatePaymentInputModel
			{
				DefaultPaymentMethodId = 1,
				DefaultPaymentDetailsId = 2
			};

			var (result, msg) = await _userService.ChangeUserPaymentDataAsync(model, "userId");
			Assert.That(result, Is.False);
		}

		[Test]
		public async Task ChangeUserPaymentDataAsync_NoChanges_ReturnsFalseWithMessage()
		{
			var user = new ApplicationUser
			{
				DefaultPaymentMethodId = 1,
				DefaultPaymentDetailsId = 2
			};

			_mockUserManager.Setup(m => m.FindByIdAsync("userId"))
				.ReturnsAsync(user);

			var model = new UpdatePaymentInputModel
			{
				DefaultPaymentMethodId = 1,
				DefaultPaymentDetailsId = 2
			};

			var (result, msg) = await _userService.ChangeUserPaymentDataAsync(model, "userId");

			Assert.That(result, Is.False);
			StringAssert.Contains("The User has already this payment data as default!", msg);
		}

		[Test]
		public async Task ChangeUserPaymentDataAsync_ChangesDetected_UpdatesUser_ReturnsSuccess()
		{
			var user = new ApplicationUser
			{
				DefaultPaymentMethodId = 1,
				DefaultPaymentDetailsId = 2
			};

			_mockUserManager.Setup(m => m.FindByIdAsync("userId"))
				.ReturnsAsync(user);

			_mockUserManager.Setup(m => m.UpdateAsync(user))
				.ReturnsAsync(IdentityResult.Success);

			var model = new UpdatePaymentInputModel
			{
				DefaultPaymentMethodId = 3,
				DefaultPaymentDetailsId = 4
			};

			var (result, msg) = await _userService.ChangeUserPaymentDataAsync(model, "userId");

			Assert.That(result, Is.True);
			StringAssert.Contains("successfully updated", msg);

			_mockUserManager.Verify(m => m.UpdateAsync(It.Is<ApplicationUser>(u =>
				u.DefaultPaymentMethodId == 3 &&
				u.DefaultPaymentDetailsId == 4)), Times.Once);
		}

		#endregion
	}
}

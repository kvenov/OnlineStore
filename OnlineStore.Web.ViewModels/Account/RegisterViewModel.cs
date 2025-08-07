using System.ComponentModel.DataAnnotations;

using static OnlineStore.Common.ApplicationConstants.AddressValidationConstants;
using static OnlineStore.Common.ErrorMessages.AddressValidationMessages;

namespace OnlineStore.Web.ViewModels.Account
{
	public class RegisterViewModel
	{

		[Required]
		public string FirstName { get; set; } = null!;

		[Required]
		public string LastName { get; set; } = null!;

		[Required(ErrorMessage = Required)]
		[EmailAddress(ErrorMessage = EmailInvalid)]
		[StringLength(EmailMaxLength, ErrorMessage = EmailLength)]
		public string Email { get; set; } = null!;

		[Required]
		public string Username { get; set; } = null!;

		[Required, DataType(DataType.Password)]
		public string Password { get; set; } = null!;

		[Required, DataType(DataType.Password), Compare(nameof(Password))]
		public string ConfirmPassword { get; set; } = null!;

		public string? ReturnUrl { get; set; }
	}
}

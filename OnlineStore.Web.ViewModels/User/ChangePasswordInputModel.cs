using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Web.ViewModels.User
{
	public class ChangePasswordInputModel
	{

		[Required(ErrorMessage = "Please enter your current password.")]
		[DataType(DataType.Password)]
		public string CurrentPassword { get; set; } = null!;

		[Required(ErrorMessage = "Please enter a new password.")]
		[StringLength(100, MinimumLength = 6, ErrorMessage = "The password must be at least 6 characters.")]
		[DataType(DataType.Password)]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
			ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one number.")]
		public string NewPassword { get; set; } = null!;

		[Required(ErrorMessage = "Please confirm your new password.")]
		[DataType(DataType.Password)]
		[Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
		public string ConfirmNewPassword { get; set; } = null!;
	}
}

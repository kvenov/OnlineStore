using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Web.ViewModels.Account
{
	public class LoginViewModel
	{

		[Required(ErrorMessage = "Email or Username is required")]
		[Display(Name = "Email or Username")]
		public string LoginInput { get; set; } = null!;

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; } = null!;

		[Display(Name = "Remember me?")]
		public bool RememberMe { get; set; }

		public string ReturnUrl { get; set; } = null!;
	}
}

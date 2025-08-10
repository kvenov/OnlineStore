using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Web.ViewModels.User
{
	public class UpdateProfileInputModel
	{

		[StringLength(100)]
		[Display(Name = "Full Name")]
		public string? FullName { get; set; }

		[StringLength(100)]
		[Display(Name = "Username")]
		public string? Username { get; set; }

		[EmailAddress]
		[Display(Name = "Email")]
		public string? Email { get; set; }

		[Phone]
		[Display(Name = "Phone Number")]
		public string? PhoneNumber { get; set; }
	}
}

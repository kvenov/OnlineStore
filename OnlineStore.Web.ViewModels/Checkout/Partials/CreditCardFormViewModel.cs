using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Web.ViewModels.Checkout.Partials
{
	public class CreditCardFormViewModel
	{

		[Required]
		[Display(Name = "Name on Card")]
		public string NameOnCard { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Card Number")]
		public string CardNumber { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Expiry Month")]
		public int ExpMonth { get; set; }

		[Required]
		[Display(Name = "Expiry Year")]
		public int ExpYear { get; set; }

		[Required]
		[Display(Name = "CVV")]
		public string CVV { get; set; } = string.Empty;
	}
}

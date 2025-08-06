namespace OnlineStore.Web.ViewModels.Order
{
	public class OrderPaymentDetailsViewModel
	{

		public string CardNumberMasked { get; set; } = null!;

		public string CardBrand { get; set; } = null!;

		public int ExpMonth { get; set; }

		public int ExpYear { get; set; }

		public string NameOnCard { get; set; } = null!;

		public string PaymentStatus { get; set; } = null!;
	}
}

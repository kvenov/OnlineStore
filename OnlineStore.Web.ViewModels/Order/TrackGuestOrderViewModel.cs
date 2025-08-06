using System.ComponentModel.DataAnnotations;

using static OnlineStore.Common.ApplicationConstants.AddressValidationConstants;
using static OnlineStore.Common.ErrorMessages.AddressValidationMessages;

namespace OnlineStore.Web.ViewModels.Order
{
	public class TrackGuestOrderViewModel
	{
		[Required(ErrorMessage = Required)]
		[EmailAddress(ErrorMessage = EmailInvalid)]
		[StringLength(EmailMaxLength, ErrorMessage = EmailLength)]
		public string GuestEmail { get; set; } = null!;

		[Required]
		public string OrderNumber { get; set; } = null!;

		public bool HasSearched { get; set; } = false;
		public bool OrderFound { get; set; } = false;

		public string? Status { get; set; }
		public decimal TotalAmount { get; set; }
		public string? ShippingOption { get; set; }
		public string? EstimatedDeliveryStartFormatted { get; set; }
		public string? EstimatedDeliveryEndFormatted { get; set; }

		public List<OrderProductViewModel> Items { get; set; } = new();
	}
}

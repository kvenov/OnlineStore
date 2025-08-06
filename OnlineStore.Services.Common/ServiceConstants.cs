using OnlineStore.Data.Models.Enums;

namespace OnlineStore.Services.Common
{
	public static class ServiceConstants
	{

		public const PaymentStatus DefaultStartingPaymentStatus = PaymentStatus.Pending;

		public const OrderStatus DefaultStartingOrderStatus = OrderStatus.Pending;
	}
}

namespace OnlineStore.Services.Core.DTO.Sales.OrderManagement
{
	public class OrderFilterDto
	{
		public string? OrderNumber { get; set; }
		public string? Customer { get; set; }
		public DateTime? DateFrom { get; set; }
		public DateTime? DateTo { get; set; }
		public string? Status { get; set; }
	}
}

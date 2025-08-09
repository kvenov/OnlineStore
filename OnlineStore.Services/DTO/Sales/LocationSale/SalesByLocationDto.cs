namespace OnlineStore.Services.Core.DTO.Sales.LocationSale
{
	public class SalesByLocationDto
	{
		public DateTime? FromDate { get; set; }
		public DateTime? ToDate { get; set; }
		public string? Country { get; set; }

		public string? City { get; set; }
		public string? ZipCode { get; set; }
	}
}

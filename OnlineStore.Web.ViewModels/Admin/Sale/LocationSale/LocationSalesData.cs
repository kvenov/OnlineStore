namespace OnlineStore.Web.ViewModels.Admin.Sale.LocationSales
{
	public class LocationSalesData
	{
		public string LocationName { get; set; } = null!;
		public decimal TotalSales { get; set; }
		public int OrdersCount { get; set; }

		public double? Latitude { get; set; }
		public double? Longitude { get; set; }
	}
}
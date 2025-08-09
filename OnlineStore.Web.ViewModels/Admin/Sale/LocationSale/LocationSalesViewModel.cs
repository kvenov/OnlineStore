namespace OnlineStore.Web.ViewModels.Admin.Sale.LocationSales
{
	public class LocationSalesViewModel
	{
		public List<LocationSalesData> SalesByCountry { get; set; } = new();
		public List<LocationSalesData> SalesByCity { get; set; } = new();
	}
}

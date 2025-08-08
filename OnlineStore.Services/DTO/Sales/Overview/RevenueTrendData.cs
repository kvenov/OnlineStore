namespace OnlineStore.Services.Core.DTO.Sales.Overview
{
	public class RevenueTrendData
	{
		public List<string> Labels { get; set; } = new();
		public List<decimal> Values { get; set; } = new();
	}
}
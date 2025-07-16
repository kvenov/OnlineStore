namespace OnlineStore.Services.Core.DTO.Product
{
	public class GetSearchedProductsDto
	{
		public int Id { get; set; }

		public string Name { get; set; } = null!;

		public string ImageUrl { get; set; } = null!;

		public string Price { get; set; } = null!;

	}
}

using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using static OnlineStore.Data.Common.Constants.EntityConstants.ShoppingCartItem;

namespace OnlineStore.Data.DTOs
{

	[XmlType("ShoppingCartItem")]
	public class ImportShoppingCartItemDTO
	{

		[Required]
		[MinLength(ShoppingCartItemQuantityMinValue)]
		[XmlElement(nameof(Quantity))]
		public string Quantity { get; set; } = null!;

		[Required]
		[XmlElement(nameof(Price))]
		public string Price { get; set; } = null!;

		[Required]
		[XmlElement(nameof(TotalPrice))]
		public string TotalPrice { get; set; } = null!;

		[Required]
		[XmlElement(nameof(ProductId))]
		public string ProductId { get; set; } = null!;
	}
}

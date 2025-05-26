using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace OnlineStore.Data.DTOs
{

	[XmlType("ShoppingCart")]
	public class ImportShoppingCartDTO
	{

		[Required]
		[XmlElement(nameof(CreatedAt))]
		public string CreatedAt { get; set; } = null!;

		[Required]
		[XmlElement(nameof(UserId))]
		public string UserId { get; set; } = null!;

		[XmlArray(nameof(ShoppingCartItems))]
		[XmlArrayItem("ShoppingCartItem")]
		public ImportShoppingCartItemDTO[] ShoppingCartItems { get; set; } = null!;
	}
}

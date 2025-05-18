using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using static OnlineStore.Common.Constants.EntityConstants.ProductRating;

namespace OnlineStore.Data.DTOs
{

	[XmlType("ProductRating")]
	public class ImportProductRatingDTO
	{

		[Required]
		[XmlElement(nameof(ProductId))]
		public string ProductId { get; set; } = null!;

		[XmlElement(nameof(UserId))]
		public string? UserId { get; set; }

		[Required]
		[XmlElement(nameof(Rating))]
		[MaxLength(ProductRatingMaxValue)]
		public string Rating { get; set; } = null!;

		[XmlElement(nameof(Review))]
		[MaxLength(ProductRatingReviewMaxLength)]
		public string? Review { get; set; }

		[Required]
		[XmlElement(nameof(CreatedAt))]
		public string CreatedAt { get; set; } = null!;

		[Required]
		[XmlElement(nameof(IsDeleted))]
		public string IsDeleted { get; set; } = null!;
	}
}

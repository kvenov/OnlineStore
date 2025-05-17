using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using static OnlineStore.Common.Constants.EntityConstants.ArticleCategory;

namespace OnlineStore.Data.DTOs
{

	[XmlType("ArticleCategory")]
	public class ArticleCategoryDTO
	{

		[Required]
		[XmlElement(nameof(Name))]
		[MaxLength(ArticleCategoryNameMaxLength)]
		public string Name { get; set; } = null!;

		[XmlElement(nameof(Description))]
		[MaxLength(ArticleCategoryDescriptionMaxLength)]
		public string? Description { get; set; }

		[Required]
		[XmlElement(nameof(IsDeleted))]
		public string IsDeleted { get; set; } = null!;
	}
}

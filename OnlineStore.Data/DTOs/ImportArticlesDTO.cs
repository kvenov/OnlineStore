using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using static OnlineStore.Data.Common.Constants.EntityConstants.Article;

namespace OnlineStore.Data.DTOs
{

	[XmlType("Article")]
	public class ImportArticlesDTO
	{
		[Required]
		[XmlElement(nameof(Title))]
		[MaxLength(ArticleTitleMaxLength)]
		public string Title { get; set; } = null!;

		[Required]
		[XmlElement(nameof(Content))]
		public string Content { get; set; } = null!;

		[Required]
		[XmlElement(nameof(PublishedDate))]
		public string PublishedDate { get; set; } = null!;

		[XmlElement(nameof(ImageUrl))]
		[MaxLength(ArticleImageUrlMaxLenth)]
		public string? ImageUrl { get; set; }

		[Required]
		[XmlElement(nameof(IsPublished))]
		public string IsPublished { get; set; } = null!;

		[XmlElement(nameof(AuthorId))]
		public string? AuthorId { get; set; }

		[Required]
		[XmlElement(nameof(CategoryId))]
		public string CategoryId { get; set; } = null!;

		[Required]
		[XmlElement(nameof(IsDeleted))]
		public string IsDeleted { get; set; } = null!;
	}
}

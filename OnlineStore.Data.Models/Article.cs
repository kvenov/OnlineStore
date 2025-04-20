using Microsoft.EntityFrameworkCore;

namespace OnlineStore.Data.Models
{

	[Comment("Articles in the store")]
	public class Article
	{

		[Comment("Article identifier")]
		public int Id { get; set; }

		[Comment("Article title")]
		public string Title { get; set; } = null!;

		[Comment("Article content")]
		public string Content { get; set; } = null!;

		[Comment("Article date of creation")]
		public DateTime PublishedDate { get; set; }

		[Comment("Article image url")]
		public string? ImageUrl { get; set; }

		[Comment("Article short info")]
		public bool IsPublished { get; set; }



		public string? AuthorId { get; set; }

		[Comment("Article author")]
		public virtual ApplicationUser? Author { get; set; }

		public int CategoryId { get; set; }

		[Comment("Article category")]
		public virtual ArticleCategory Category { get; set; } = null!;
	}
}

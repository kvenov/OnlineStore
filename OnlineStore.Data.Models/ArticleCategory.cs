using Microsoft.EntityFrameworkCore;

namespace OnlineStore.Data.Models
{

	[Comment("Article categories in the store")]
	public class ArticleCategory
	{

		[Comment("Article category identifier")]
		public int Id { get; set; }

		[Comment("Article category name")]
		public string Name { get; set; } = null!;

		[Comment("Article category description")]
		public string? Description { get; set; }

		[Comment("Articles into the category")]
		public virtual ICollection<Article> Articles { get; set; } = 
				new HashSet<Article>();
	}
}

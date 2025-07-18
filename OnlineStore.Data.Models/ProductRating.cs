﻿using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models.Interfaces;

namespace OnlineStore.Data.Models
{

	[Comment("The Products ratings in the store")]
	public class ProductRating : ISoftDeletable
	{

		[Comment("Product rating identifier")]
		public int Id { get; set; }

		public int ProductId { get; set; }

		[Comment("Product that the rating belongs to")]
		public virtual Product Product { get; set; } = null!;

		public string UserId { get; set; } = null!;

		[Comment("User that made the current rating")]
		public virtual ApplicationUser User { get; set; } = null!;

		[Comment("Rating value")]
		public int Rating { get; set; }

		[Comment("Rating creation date")]
		public DateTime CreatedAt { get; set; }

		public bool IsDeleted { get; set; }
	}
}

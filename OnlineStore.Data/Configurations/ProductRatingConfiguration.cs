﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;
using static OnlineStore.Data.Common.Constants.EntityConstants.ProductRating;

namespace OnlineStore.Data.Configurations
{
	public class ProductRatingConfiguration : IEntityTypeConfiguration<ProductRating>
	{
		public void Configure(EntityTypeBuilder<ProductRating> entity)
		{

			entity
				.HasKey(p => p.Id);

			entity
				.Property(p => p.ProductId)
				.IsRequired(true);

			entity
				.Property(p => p.UserId)
				.IsRequired(true);

			entity
				.Property(p => p.Rating)
				.HasDefaultValue(0)
				.HasMaxLength(ProductRatingMaxValue)
				.IsRequired(true);

			entity
				.Property(pr => pr.IsDeleted)
				.HasDefaultValue(false)
				.IsRequired(true);

			entity
				.Property(p => p.CreatedAt)
				.HasDefaultValueSql("GETDATE()")
				.IsRequired(true);

			entity
				.HasIndex(p => new { p.ProductId, p.UserId })
				.IsUnique(true);

			entity
				.HasOne(pr => pr.Product)
				.WithMany(p => p.ProductRatings)
				.HasForeignKey(pr => pr.ProductId)
				.OnDelete(DeleteBehavior.Cascade);

			entity
				.HasOne(pr => pr.User)
				.WithMany(u => u.ProductRatings)
				.HasForeignKey(pr => pr.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			entity
				.HasQueryFilter(pr => pr.IsDeleted == false && 
									  pr.Product.IsDeleted == false && 
									  pr.User.IsDeleted == false);

		}
	}
}

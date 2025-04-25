using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;
using static OnlineStore.Common.EntityConstants.ProductDetails;

namespace OnlineStore.Data.Configurations
{
	public class ProductDetailsConfiguration : IEntityTypeConfiguration<ProductDetails>
	{
		public void Configure(EntityTypeBuilder<ProductDetails> entity)
		{

			entity
				.HasKey(p => p.Id);

			entity
				.Property(p => p.Material)
				.IsRequired()
				.HasMaxLength(MaterialMaxLength)
				.IsUnicode(true);

			entity
				.Property(p => p.Color)
				.IsRequired()
				.HasMaxLength(ColorMaxLength);

			entity
				.Property(p => p.Gender)
				.IsRequired()
				.HasMaxLength(GenderMaxLength);

			entity
				.Property(p => p.SizeGuideUrl)
				.IsRequired()
				.HasMaxLength(SizeGuideUrlMaxLength)
				.IsUnicode(false);

			entity
				.Property(p => p.CountryOfOrigin)
				.IsRequired()
				.HasMaxLength(CountryOfOriginMaxLength);

			entity
				.Property(p => p.CareInstructions)
				.IsRequired()
				.HasMaxLength(CareInstructionsMaxLength)
				.IsUnicode(true);

			entity
				.Property(p => p.Weight)
				.HasMaxLength(MaxWeightKg)
				.IsRequired();

			entity
				.Property(p => p.Fit)
				.IsRequired()
				.HasMaxLength(FitMaxLength)
				.IsUnicode(true);

			entity
				.Property(p => p.Style)
				.IsRequired()
				.HasMaxLength(StyleMaxLength)
				.IsUnicode(true);

			entity
				.Property(p => p.ProductId)
				.IsRequired(true);

			entity
				.HasOne(pd => pd.Product)
				.WithOne(p => p.ProductDetails)
				.HasForeignKey<ProductDetails>(pd => pd.ProductId)
				.OnDelete(DeleteBehavior.Cascade);

		}
	}
}

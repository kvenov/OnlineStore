using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;
using static OnlineStore.Data.Common.Constants.EntityConstants.ProductDetails;

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
				.Property(p => p.Size)
				.IsRequired();

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
				.HasPrecision(18, 2)
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

			entity
				.HasQueryFilter(wi => wi.Product.IsDeleted == false);

			entity
				.HasData(this.GenerateProductDetails());
		}

		private IEnumerable<ProductDetails> GenerateProductDetails()
		{
			return new List<ProductDetails>()
			{
				new ProductDetails
				{
					Id = 10,
					Material = "Polyester",
					Color = "Black",
					Gender = "Men",
					Size = "L",
					SizeGuideUrl = "https://example.com/size-guide",
					CountryOfOrigin = "Vietnam",
					CareInstructions = "Machine wash cold, tumble dry low.",
					Weight = 0.45m,
					Fit = "Regular",
					Style = "Sport",
					ProductId = 12
				},
				new ProductDetails
				{
					Id = 11,
					Material = "Textile",
					Color = "White",
					Gender = "Unisex",
					Size = "42 EU",
					SizeGuideUrl = "https://example.com/size-guide",
					CountryOfOrigin = "Indonesia",
					CareInstructions = "Spot clean with mild detergent.",
					Weight = 0.9m,
					Fit = "Regular",
					Style = "Running",
					ProductId = 13
				},
				new ProductDetails
				{
					Id = 12,
					Material = "Denim",
					Color = "Indigo",
					Gender = "Men",
					Size = "32/32",
					SizeGuideUrl = "https://example.com/size-guide",
					CountryOfOrigin = "Mexico",
					CareInstructions = "Wash inside out with like colors.",
					Weight = 0.75m,
					Fit = "Slim",
					Style = "Casual",
					ProductId = 14
				},
				new ProductDetails
				{
					Id = 13,
					Material = "Fleece",
					Color = "Grey",
					Gender = "Men",
					Size = "M",
					SizeGuideUrl = "https://example.com/size-guide",
					CountryOfOrigin = "China",
					CareInstructions = "Machine wash warm.",
					Weight = 0.65m,
					Fit = "Regular",
					Style = "Streetwear",
					ProductId = 15
				},
				new ProductDetails
				{
					Id = 14,
					Material = "Cotton",
					Color = "Black",
					Gender = "Men",
					Size = "L",
					SizeGuideUrl = "https://example.com/size-guide",
					CountryOfOrigin = "Thailand",
					CareInstructions = "Do not bleach.",
					Weight = 0.35m,
					Fit = "Athletic",
					Style = "Sport",
					ProductId = 16
				},
				new ProductDetails
				{
					Id = 15,
					Material = "Cotton",
					Color = "White",
					Gender = "Unisex",
					Size = "M",
					SizeGuideUrl = "https://example.com/size-guide",
					CountryOfOrigin = "Bangladesh",
					CareInstructions = "Machine wash cold, iron if needed.",
					Weight = 0.4m,
					Fit = "Classic",
					Style = "Casual",
					ProductId = 17
				},
				new ProductDetails
				{
					Id = 16,
					Material = "Cotton Blend",
					Color = "Black",
					Gender = "Unisex",
					Size = "M",
					SizeGuideUrl = "https://example.com/size-guide",
					CountryOfOrigin = "India",
					CareInstructions = "Wash in cold water.",
					Weight = 0.15m,
					Fit = "Tight",
					Style = "Training",
					ProductId = 18
				},
				new ProductDetails
				{
					Id = 17,
					Material = "Polyester",
					Color = "Blue",
					Gender = "Unisex",
					Size = "One Size",
					SizeGuideUrl = "https://example.com/size-guide",
					CountryOfOrigin = "China",
					CareInstructions = "Hand wash.",
					Weight = 0.6m,
					Fit = "Standard",
					Style = "Urban",
					ProductId = 19
				},
				new ProductDetails
				{
					Id = 18,
					Material = "Cotton",
					Color = "Grey",
					Gender = "Men",
					Size = "M",
					SizeGuideUrl = "https://www.nike.com/size-fit/men",
					CountryOfOrigin = "Vietnam",
					CareInstructions = "Machine wash cold",
					Weight = 0.65m,
					Fit = "Regular",
					Style = "Pullover",
					ProductId = 20
				},
				new ProductDetails
				{
					Id = 19,
					Material = "Cotton",
					Color = "Black",
					Gender = "Men",
					Size = "L",
					SizeGuideUrl = "https://www.levi.com/size-guide",
					CountryOfOrigin = "Bangladesh",
					CareInstructions = "Machine wash",
					Weight = 0.3m,
					Fit = "Relaxed",
					Style = "Crewneck",
					ProductId = 21
				},
				new ProductDetails
				{
					Id = 20,
					Material = "Polyester",
					Color = "Navy",
					Gender = "Men",
					Size = "L",
					SizeGuideUrl = "https://www.nike.com/size-fit/men",
					CountryOfOrigin = "Thailand",
					CareInstructions = "Machine wash cold",
					Weight = 0.25m,
					Fit = "Regular",
					Style = "Athletic",
					ProductId = 22
				},
				new ProductDetails
				{
					Id = 21,
					Material = "Fleece",
					Color = "Grey",
					Gender = "Unisex",
					Size = "M",
					SizeGuideUrl = "https://www.adidas.com/us/help/size_charts",
					CountryOfOrigin = "Indonesia",
					CareInstructions = "Machine wash warm",
					Weight = 0.7m,
					Fit = "Loose",
					Style = "Pullover",
					ProductId = 23
				},
				new ProductDetails
				{
					Id = 22,
					Material = "Cotton/Polyester",
					Color = "Blue",
					Gender = "Unisex",
					Size = "M",
					SizeGuideUrl = "https://www.nike.com/size-fit/unisex",
					CountryOfOrigin = "China",
					CareInstructions = "Machine wash cold",
					Weight = 0.15m,
					Fit = "Snug",
					Style = "Low cut",
					ProductId = 24
				},
				new ProductDetails
				{
					Id = 23,
					Material = "Denim",
					Color = "Blue",
					Gender = "Men",
					Size = "L",
					SizeGuideUrl = "https://www.levi.com/size-guide",
					CountryOfOrigin = "Mexico",
					CareInstructions = "Machine wash cold",
					Weight = 1.2m,
					Fit = "Regular",
					Style = "Trucker",
					ProductId = 25
				},
				new ProductDetails
				{
					Id = 24,
					Material = "Textile",
					Color = "White",
					Gender = "Men",
					Size = "42",
					SizeGuideUrl = "https://www.adidas.com/us/help/size_charts",
					CountryOfOrigin = "Vietnam",
					CareInstructions = "Wipe with damp cloth",
					Weight = 0.9m,
					Fit = "True to size",
					Style = "Running",
					ProductId = 26
				},
				new ProductDetails
				{
					Id = 25,
					Material = "Polyester",
					Color = "Red",
					Gender = "Unisex",
					Size = "18L",
					SizeGuideUrl = "https://www.nike.com/size-fit/bags",
					CountryOfOrigin = "China",
					CareInstructions = "Wipe clean",
					Weight = 0.8m,
					Fit = "Compact",
					Style = "Training",
					ProductId = 27
				},
				new ProductDetails
				{
					Id = 26,
					Material = "Leather",
					Color = "Black",
					Gender = "Men",
					Size = "43",
					SizeGuideUrl = "https://www.nike.com/size-fit/mens-shoes",
					CountryOfOrigin = "Vietnam",
					CareInstructions = "Wipe with damp cloth",
					Weight = 1.0m,
					Fit = "True to size",
					Style = "Lifestyle",
					ProductId = 28
				},
				new ProductDetails
				{
					Id = 27,
					Material = "Denim",
					Color = "Blue",
					Gender = "Men",
					Size = "32",
					SizeGuideUrl = "https://www.levi.com/size-guide",
					CountryOfOrigin = "Mexico",
					CareInstructions = "Machine wash cold",
					Weight = 1.1m,
					Fit = "Slim",
					Style = "Jeans",
					ProductId = 29
				},
				new ProductDetails
				{
					Id = 28,
					Material = "Polyester",
					Color = "Black",
					Gender = "Men",
					Size = "M",
					SizeGuideUrl = "https://www.nike.com/size-fit/mens-tops",
					CountryOfOrigin = "Thailand",
					CareInstructions = "Machine wash cold",
					Weight = 0.25m,
					Fit = "Athletic",
					Style = "Training",
					ProductId = 30
				},
				new ProductDetails
				{
					Id = 29,
					Material = "Cotton",
					Color = "Black",
					Gender = "Female",
					Size = "S",
					SizeGuideUrl = "https://www.nike.com/size-fit",
					CountryOfOrigin = "Vietnam",
					CareInstructions = "Machine wash cold",
					Weight = 0.25m,
					Fit = "Relaxed",
					Style = "Casual",
					ProductId = 31
				},
				new ProductDetails
				{
					Id = 30,
					Material = "Polyester",
					Color = "White",
					Gender = "Female",
					Size = "M",
					SizeGuideUrl = "https://www.adidas.com/size-chart",
					CountryOfOrigin = "Cambodia",
					CareInstructions = "Wash with similar colors",
					Weight = 0.20m,
					Fit = "Athletic",
					Style = "Sport",
					ProductId = 32
				},
				new ProductDetails
				{
					Id = 31,
					Material = "Denim",
					Color = "Blue",
					Gender = "Female",
					Size = "M",
					SizeGuideUrl = "https://www.levi.com/size-guide",
					CountryOfOrigin = "Mexico",
					CareInstructions = "Wash inside out",
					Weight = 0.60m,
					Fit = "Skinny",
					Style = "Streetwear",
					ProductId = 33
				},
				new ProductDetails
				{
					Id = 32,
					Material = "Cotton Blend",
					Color = "Pink",
					Gender = "Female",
					Size = "L",
					SizeGuideUrl = "https://www.nike.com/size-fit",
					CountryOfOrigin = "Indonesia",
					CareInstructions = "Do not bleach",
					Weight = 0.50m,
					Fit = "Loose",
					Style = "Lounge",
					ProductId = 34
				},
				new ProductDetails
				{
					Id = 33,
					Material = "Mesh",
					Color = "Grey",
					Gender = "Female",
					Size = "38",
					SizeGuideUrl = "https://www.nike.com/size-fit",
					CountryOfOrigin = "China",
					CareInstructions = "Clean with soft brush",
					Weight = 0.70m,
					Fit = "Regular",
					Style = "Running",
					ProductId = 35
				},
				new ProductDetails
				{
					Id = 34,
					Material = "Cotton",
					Color = "Peach",
					Gender = "Female",
					Size = "S",
					SizeGuideUrl = "https://www.adidas.com/size-chart",
					CountryOfOrigin = "Bangladesh",
					CareInstructions = "Tumble dry low",
					Weight = 0.22m,
					Fit = "Regular",
					Style = "Casual",
					ProductId = 36
				},
				new ProductDetails
				{
					Id = 35,
					Material = "Polyester",
					Color = "Black",
					Gender = "Female",
					Size = "M",
					SizeGuideUrl = "https://www.nike.com/size-fit",
					CountryOfOrigin = "Vietnam",
					CareInstructions = "Do not iron",
					Weight = 0.30m,
					Fit = "Athletic",
					Style = "Training",
					ProductId = 37
				},
				new ProductDetails
				{
					Id = 36,
					Material = "Denim",
					Color = "Dark Blue",
					Gender = "Female",
					Size = "M",
					SizeGuideUrl = "https://www.levi.com/size-guide",
					CountryOfOrigin = "India",
					CareInstructions = "Cold wash only",
					Weight = 0.65m,
					Fit = "Super Skinny",
					Style = "Urban",
					ProductId = 38
				},
				new ProductDetails
				{
					Id = 37,
					Material = "Synthetic Leather",
					Color = "White",
					Gender = "Female",
					Size = "39",
					SizeGuideUrl = "https://www.adidas.com/size-chart",
					CountryOfOrigin = "Indonesia",
					CareInstructions = "Wipe clean",
					Weight = 0.75m,
					Fit = "Regular",
					Style = "Lifestyle",
					ProductId = 39
				},
				new ProductDetails
				{
					Id = 38,
					Material = "Nylon",
					Color = "Black",
					Gender = "Female",
					Size = "M",
					SizeGuideUrl = "https://www.nike.com/size-fit",
					CountryOfOrigin = "Vietnam",
					CareInstructions = "Hand wash only",
					Weight = 0.45m,
					Fit = "Relaxed",
					Style = "Windbreaker",
					ProductId = 40
				}
			};
		}
	}
}

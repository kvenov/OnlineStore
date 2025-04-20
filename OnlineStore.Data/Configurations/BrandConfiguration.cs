using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;
using static OnlineStore.Common.EntityConstants.Brand;

namespace OnlineStore.Data.Configurations
{
	public class BrandConfiguration : IEntityTypeConfiguration<Brand>
	{
		public void Configure(EntityTypeBuilder<Brand> entity)
		{

			entity
				.HasKey(a => a.Id);

			entity
				.Property(p => p.Name)
				.IsRequired(true)
				.HasMaxLength(BrandNameMaxLength);

			entity
				.Property(p => p.Description)
				.IsRequired(false)
				.IsUnicode(true)
				.HasMaxLength(BrandDescriptionMaxLength);

			entity
				.Property(p => p.LogoUrl)
				.IsRequired(true)
				.IsUnicode(true)
				.HasMaxLength(BrandLogoUrlMaxLength);

			entity
				.Property(p => p.WebsiteUrl)
				.IsRequired(true)
				.IsUnicode(true)
				.HasMaxLength(BrandWebsiteUrlMaxLength);

			entity
				.Property(p => p.IsActive)
				.IsRequired(true);
		}
	}
}

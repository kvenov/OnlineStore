using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;
using static OnlineStore.Data.Common.Constants.EntityConstants.Address;

namespace OnlineStore.Data.Configurations
{
	public class AddressConfiguration : IEntityTypeConfiguration<Address>
	{
		public void Configure(EntityTypeBuilder<Address> entity)
		{

			entity
				.HasKey(p => p.Id);

			entity
				.Property(p => p.Street)
				.IsRequired()
				.IsUnicode()
				.HasMaxLength(AddressStreetMaxLength);

			entity
				.Property(p => p.City)
				.IsRequired()
				.IsUnicode()
				.HasMaxLength(AddressCityMaxLength);

			entity
				.Property(p => p.Country)
				.IsRequired()
				.IsUnicode()
				.HasMaxLength(AddressCountryMaxLength);

			entity
				.Property(p => p.ZipCode)
				.IsRequired()
				.IsUnicode()
				.HasMaxLength(AddressZipCodeMaxLength);

			entity
				.Property(p => p.PhoneNumber)
				.IsRequired()
				.IsUnicode()
				.HasMaxLength(AddressPhoneNumberMaxLength);

			entity
				.Property(p => p.IsBillingAddress)
				.IsRequired();

			entity
				.Property(p => p.IsShippingAddress)
				.IsRequired();

			entity
				.Property(a => a.UserId)
				.IsRequired(false);

			entity
				.Property(a => a.GuestId)
				.IsRequired(false);

			entity
				.HasOne(a => a.User)
				.WithMany(u => u.Addresses)
				.HasForeignKey(a => a.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			entity
				.HasIndex(p => p.UserId);

			entity
				.HasIndex(p => p.GuestId);

			entity
				.HasQueryFilter(a => (a.User != null && !a.User.IsDeleted) ||
									 (a.User == null && a.GuestId != null));
		}
	}
}

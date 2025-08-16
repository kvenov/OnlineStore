using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;
using static OnlineStore.Data.Common.Constants.EntityConstants.PaymentDetails;

namespace OnlineStore.Data.Configurations
{
	public class PaymentDetailsConfiguration : IEntityTypeConfiguration<PaymentDetails>
	{
		public void Configure(EntityTypeBuilder<PaymentDetails> entity)
		{
			entity
				.HasKey(p => p.Id);

			entity
				.Property(p => p.CardNumber)
				.HasMaxLength(PaymentDetailsCardNumberMaxLength)
				.IsRequired(true);

			entity
				.Property(p => p.CardBrand)
				.HasMaxLength(PaymentDetailsCardBrandMaxLength)
				.IsRequired(false);

			entity 
				.Property(p => p.ExpMonth)
				.HasPrecision(2)
				.IsRequired(true);

			entity 
				.Property(p => p.ExpYear)
				.HasPrecision(4)
				.IsRequired(true);

			entity
				.Property(p => p.NameOnCard)
				.HasMaxLength(PaymentDetailsNameOnCardMaxLength)
				.IsRequired(true);

			entity
				.Property(p => p.Status)
				.IsRequired(true);

			entity
				.HasIndex(p => p.Status);

		}
	}
	
}

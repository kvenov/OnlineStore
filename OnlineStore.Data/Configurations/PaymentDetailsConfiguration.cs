using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;
using static OnlineStore.Common.EntityConstants.PaymentDetails;

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
				.IsRequired(false);

			entity 
				.Property(p => p.ExpYear)
				.IsRequired(false);

			entity
				.Property(p => p.NameOnCard)
				.HasMaxLength(PaymentDetailsNameOnCardMaxLength)
				.IsRequired();

			entity
				.Property(p => p.PaidAt)
				.IsRequired(false);

			entity
				.Property(p => p.Status)
				.IsRequired(true);

			entity
				.Property(p => p.OrderId)
				.IsRequired(true);

			entity
				.HasOne(p => p.Order)
				.WithOne(o => o.PaymentDetails)
				.HasForeignKey<PaymentDetails>(p => p.OrderId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
	
}

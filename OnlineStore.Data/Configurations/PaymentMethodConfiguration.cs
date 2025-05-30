﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;
using static OnlineStore.Data.Common.Constants.EntityConstants.PaymentMethod;

namespace OnlineStore.Data.Configurations
{
	public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
	{
		public void Configure(EntityTypeBuilder<PaymentMethod> entity)
		{
			
			entity
				.HasKey(p => p.Id);

			entity
				.Property(p => p.Name)
				.HasMaxLength(PaymentMethodNameMaxLength)
				.IsRequired(true);

			entity
				.Property(p => p.Code)
				.IsRequired(false);

			entity
				.Property(p => p.IsActive)
				.IsRequired(true)
				.HasDefaultValue(true);

			entity
				.HasIndex(p => p.Name);
		}
	}
}

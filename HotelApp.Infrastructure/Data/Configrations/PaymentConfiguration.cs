using HotelApp.Domain.Entities.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Infrastructure.Data.Configrations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> b)
        {
            b.ToTable("Payments");
            b.HasKey(x => x.Id);

            b.Property(x => x.Method).HasMaxLength(30).IsRequired();
            b.Property(x => x.Amount).HasPrecision(18, 2);
            b.Property(x => x.ConfirmationNumber).HasMaxLength(50).IsRequired();

            b.HasIndex(x => x.ConfirmationNumber).IsUnique();
        }
    }
}

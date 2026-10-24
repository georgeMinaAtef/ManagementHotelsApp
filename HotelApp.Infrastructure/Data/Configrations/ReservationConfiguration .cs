using HotelApp.Domain.Entities.Reservations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Infrastructure.Data.Configrations
{
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> b)
        {
            b.ToTable("Reservations");
            b.HasKey(x => x.Id);

            b.Property(x => x.TotalPrice).HasPrecision(18, 2);

            // حفظ الـ Enum كنص بدل رقم (أوضح في الداتابيز)
            b.Property(x => x.ReservationStatus)
             .HasConversion<string>()
             .HasMaxLength(20);

            b.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Reservations_Dates", "[CheckOutDate] > [CheckInDate]");
                t.HasCheckConstraint("CK_Reservations_Adults", "[NumberOfAdults] >= 1");
            });
        }
    }
}

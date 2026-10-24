using HotelApp.Domain.Entities.Reservations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Infrastructure.Data.Configrations
{
    // Reservations_Payment
    public class ReservationPaymentConfiguration : IEntityTypeConfiguration<ReservationPayment>
    {
        public void Configure(EntityTypeBuilder<ReservationPayment> b)
        {
            b.ToTable("Reservations_Payment");
            b.HasKey(x => new { x.ReservationId, x.PaymentId });

            b.HasOne(x => x.Reservation).WithMany(r => r.ReservationPayments)
             .HasForeignKey(x => x.ReservationId).OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Payment).WithMany(p => p.ReservationPayments)
             .HasForeignKey(x => x.PaymentId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}

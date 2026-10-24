using HotelApp.Domain.Entities.Reservations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Infrastructure.Data.Configrations
{

    // Reservations_Guest  (Made By / Make)
    public class ReservationGuestConfiguration : IEntityTypeConfiguration<ReservationGuest>
    {
        public void Configure(EntityTypeBuilder<ReservationGuest> b)
        {
            b.ToTable("Reservations_Guest");
            b.HasKey(x => new { x.ReservationId, x.GuestId });

            b.HasOne(x => x.Reservation).WithMany(r => r.ReservationGuests)
             .HasForeignKey(x => x.ReservationId).OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Guest).WithMany(g => g.ReservationGuests)
             .HasForeignKey(x => x.GuestId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}

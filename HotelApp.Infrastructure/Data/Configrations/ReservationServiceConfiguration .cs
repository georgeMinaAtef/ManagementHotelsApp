using HotelApp.Domain.Entities.Reservations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Infrastructure.Data.Configrations
{

    // ReservationService  (Provide To / Requested By)
    public class ReservationServiceConfiguration : IEntityTypeConfiguration<ReservationService>
    {
        public void Configure(EntityTypeBuilder<ReservationService> b)
        {
            b.ToTable("ReservationService");
            b.HasKey(x => new { x.ServiceId, x.ReservationId });

            b.HasOne(x => x.Reservation).WithMany(r => r.ReservationServices)
             .HasForeignKey(x => x.ReservationId).OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Service).WithMany(s => s.ReservationServices)
             .HasForeignKey(x => x.ServiceId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}

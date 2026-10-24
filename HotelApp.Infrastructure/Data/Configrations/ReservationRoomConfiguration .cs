using HotelApp.Domain.Entities.Reservations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Infrastructure.Data.Configrations
{
    // Reservations_Rooms  (Reserve)
    public class ReservationRoomConfiguration : IEntityTypeConfiguration<ReservationRoom>
    {
        public void Configure(EntityTypeBuilder<ReservationRoom> b)
        {
            b.ToTable("Reservations_Rooms");
            b.HasKey(x => new { x.ReservationId, x.RoomId });

            b.HasOne(x => x.Reservation).WithMany(r => r.ReservationRooms)
             .HasForeignKey(x => x.ReservationId).OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Room).WithMany(r => r.ReservationRooms)
             .HasForeignKey(x => x.RoomId).OnDelete(DeleteBehavior.Restrict);
        }
    }

}

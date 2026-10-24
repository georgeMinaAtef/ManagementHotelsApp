using HotelApp.Domain.Entities.Hotels;
using HotelApp.Domain.Entities.Reservations;
using HotelApp.Domain.Entities.Rooms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Infrastructure.Data.Configrations
{
    public class RoomAmenityConfiguration : IEntityTypeConfiguration<Amenity>
    {
        public void Configure(EntityTypeBuilder<Amenity> b)
        {
            b.ToTable("Amenities");
            b.HasKey(x => new { x.RoomId, x.Name });   // PK مركب

            b.Property(x => x.Name).HasMaxLength(100);

            b.HasOne(x => x.Room)
             .WithMany(r => r.Amenities)
             .HasForeignKey(x => x.RoomId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}


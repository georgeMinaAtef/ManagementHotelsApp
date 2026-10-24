using HotelApp.Domain.Entities.Rooms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Infrastructure.Data.Configrations
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> b)
        {
            b.ToTable("Rooms");
            b.HasKey(x => x.Id);

            b.Property(x => x.RoomNumber).HasMaxLength(20).IsRequired();
            b.Property(x => x.RoomType).HasMaxLength(50).IsRequired();
            b.Property(x => x.DailyRate).HasPrecision(18, 2);

            // رقم الغرفة فريد داخل نفس الفندق
            b.HasIndex(x => new { x.HotelId, x.RoomNumber }).IsUnique();

            // Belongs To : Hotel 1 ── N Rooms
            b.HasOne(x => x.Hotel)
             .WithMany(h => h.Rooms)
             .HasForeignKey(x => x.HotelId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

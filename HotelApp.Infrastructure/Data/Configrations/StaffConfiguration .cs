using HotelApp.Domain.Entities.Hotels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Infrastructure.Data.Configrations
{
    public class StaffConfiguration : IEntityTypeConfiguration<Staff>
    {
        public void Configure(EntityTypeBuilder<Staff> b)
        {
            b.ToTable("Staff");
            b.HasKey(x => x.Id);

            b.Property(x => x.FullName).HasMaxLength(150).IsRequired();
            b.Property(x => x.Position).HasMaxLength(100).IsRequired();
            b.Property(x => x.Salary).HasPrecision(18, 2);

            // Work At : Hotel 1 ── N Staff
            b.HasOne(x => x.Hotel)
             .WithMany(h => h.Staff)
             .HasForeignKey(x => x.HotelId)
             .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

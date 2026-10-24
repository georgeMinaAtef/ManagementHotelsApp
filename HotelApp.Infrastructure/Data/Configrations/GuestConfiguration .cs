using HotelApp.Domain.Entities.Guests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Infrastructure.Data.Configrations
{
    public class GuestConfiguration : IEntityTypeConfiguration<Guest>
    {
        public void Configure(EntityTypeBuilder<Guest> b)
        {
            b.ToTable("Guests");
            b.HasKey(x => x.Id);

            b.Property(x => x.FullName).HasMaxLength(150).IsRequired();
            b.Property(x => x.Nationality).HasMaxLength(60).IsRequired();
            b.Property(x => x.PassportNumber).HasMaxLength(30).IsRequired();
            b.Property(x => x.DateOfBirth).HasColumnType("date");

            b.HasIndex(x => x.PassportNumber).IsUnique();
        }
    }
}

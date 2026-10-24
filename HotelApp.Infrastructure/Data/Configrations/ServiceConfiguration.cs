using HotelApp.Domain.Entities.Hotels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Infrastructure.Data.Configrations
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> b)
        {
            b.ToTable("Services");
            b.HasKey(x => x.Id);

            b.Property(x => x.ServiceName).HasMaxLength(150).IsRequired();
            b.Property(x => x.Charge).HasPrecision(18, 2);

            // Provided By : Staff 1 ── N Services
            b.HasOne(x => x.Staff)
             .WithMany(s => s.Services)
             .HasForeignKey(x => x.StaffId)
             .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

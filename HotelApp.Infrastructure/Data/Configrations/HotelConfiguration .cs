using HotelApp.Domain.Entities.Hotels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Infrastructure.Data.Configrations
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> b)
        {
            b.ToTable("Hotels");
            b.HasKey(x => x.Id);

            b.Property(x => x.Name).HasMaxLength(150).IsRequired();
            b.Property(x => x.Address).HasMaxLength(250).IsRequired();
            b.Property(x => x.City).HasMaxLength(100).IsRequired();
            b.Property(x => x.ContactNumber).HasMaxLength(30).IsRequired();
            b.Property(x => x.StarRating).IsRequired();
            b.ToTable(t => t.HasCheckConstraint("CK_Hotels_StarRating", "[StarRating] BETWEEN 1 AND 5"));

            // Hotel (Manager) ←→ Staff : علاقة Manage (One-to-One)
            b.HasOne(x => x.Manager)
             .WithOne()
             .HasForeignKey<Hotel>(x => x.ManagerId)
             .OnDelete(DeleteBehavior.NoAction);

            b.HasIndex(x => x.ManagerId).IsUnique();   // [Unique]
        }
    }

}

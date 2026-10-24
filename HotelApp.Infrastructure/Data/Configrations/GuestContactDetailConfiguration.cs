using HotelApp.Domain.Entities.Guests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Infrastructure.Data.Configrations
{
    public class GuestContactDetailConfiguration : IEntityTypeConfiguration<GuestContactDetail>
    {
        public void Configure(EntityTypeBuilder<GuestContactDetail> b)
        {
            b.ToTable("Guest_Contact_Details");
            b.HasKey(x => new { x.GuestId, x.Detail });   // PK مركب

            b.Property(x => x.Detail).HasMaxLength(200);

            b.HasOne(x => x.Guest)
             .WithMany(g => g.ContactDetails)
             .HasForeignKey(x => x.GuestId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

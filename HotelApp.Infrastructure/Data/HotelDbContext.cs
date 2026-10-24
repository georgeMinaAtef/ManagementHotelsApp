using HotelApp.Domain.Entities.Guests;
using HotelApp.Domain.Entities.Hotels;
using HotelApp.Domain.Entities.Payments;
using HotelApp.Domain.Entities.Reservations;
using HotelApp.Domain.Entities.Rooms;
using Microsoft.EntityFrameworkCore;


namespace HotelManagement.Infrastructure.Data;

public class HotelDbContext : DbContext
{
    public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options) { }

    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<Staff> Staff => Set<Staff>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Amenity> Amenities => Set<Amenity>();
    public DbSet<Guest> Guests => Set<Guest>();
    public DbSet<GuestContactDetail> GuestContactDetails => Set<GuestContactDetail>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<ReservationRoom> ReservationRooms => Set<ReservationRoom>();
    public DbSet<ReservationGuest> ReservationGuests => Set<ReservationGuest>();
    public DbSet<ReservationService> ReservationServices => Set<ReservationService>();
    public DbSet<ReservationPayment> ReservationPayments => Set<ReservationPayment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ---- Composite keys ----
        modelBuilder.Entity<Amenity>()
            .HasKey(a => new { a.RoomId, a.Name });

        modelBuilder.Entity<GuestContactDetail>()
            .HasKey(g => new { g.GuestId, g.Detail });

        modelBuilder.Entity<ReservationRoom>()
            .HasKey(x => new { x.ReservationId, x.RoomId });

        modelBuilder.Entity<ReservationGuest>()
            .HasKey(x => new { x.ReservationId, x.GuestId });

        modelBuilder.Entity<ReservationService>()
            .HasKey(x => new { x.ServiceId, x.ReservationId });

        modelBuilder.Entity<ReservationPayment>()
            .HasKey(x => new { x.ReservationId, x.PaymentId });

        // ---- Hotel <-> Staff ----
        // "Work At": a hotel has many staff
        modelBuilder.Entity<Staff>()
            .HasOne(s => s.Hotel)
            .WithMany(h => h.Staff)
            .HasForeignKey(s => s.HotelId)
            .OnDelete(DeleteBehavior.Restrict);

        // "Manage": a hotel has exactly one manager, and a staff member manages at most one hotel
        modelBuilder.Entity<Hotel>()
            .HasOne(h => h.Manager)
            .WithOne(s => s.ManagedHotel)
            .HasForeignKey<Hotel>(h => h.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---- Rooms ----
        modelBuilder.Entity<Room>()
            .HasIndex(r => new { r.HotelId, r.RoomNumber })
            .IsUnique();

        // ---- Money columns ----
        modelBuilder.Entity<Staff>().Property(s => s.Salary).HasPrecision(18, 2);
        modelBuilder.Entity<Service>().Property(s => s.Charge).HasPrecision(18, 2);
        modelBuilder.Entity<Room>().Property(r => r.DailyRate).HasPrecision(18, 2);
        modelBuilder.Entity<Reservation>().Property(r => r.TotalPrice).HasPrecision(18, 2);
        modelBuilder.Entity<Payment>().Property(p => p.Amount).HasPrecision(18, 2);
    }
}
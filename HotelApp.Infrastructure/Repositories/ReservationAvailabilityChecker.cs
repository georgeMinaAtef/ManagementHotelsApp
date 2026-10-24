using HotelApp.Domain.Contracts;
using HotelApp.Domain.Entities.Reservations;
using HotelManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Infrastructure.Repositories
{
    // Reads the DB only (no writes, no schema change).
    public class ReservationAvailabilityChecker(HotelDbContext db) : IReservationAvailabilityChecker
    {
        // Must match the exact string you store for cancelled reservations.
        private const string CancelledStatus = "Cancelled";

        public Task<bool> IsRoomBookedAsync(int roomId, DateTime checkIn, DateTime checkOut,
            CancellationToken c = default) =>
            db.Set<Reservation>()
                .AsNoTracking()
                .AnyAsync(r =>
                    r.ReservationStatus != CancelledStatus &&
                    r.CheckInDate < checkOut &&          // existing starts before requested ends
                    r.CheckOutDate > checkIn &&          // existing ends after requested starts
                    r.ReservationRooms.Any(rr => rr.RoomId == roomId), c);
    }
}

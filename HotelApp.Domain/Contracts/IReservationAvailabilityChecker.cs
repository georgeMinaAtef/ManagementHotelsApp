using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Domain.Contracts
{
    /// <summary>
    /// Asks the DATABASE (source of truth) whether a room already has a real reservation
    /// in the requested period. Redis only protects the short window before confirmation.
    /// </summary>
    public interface IReservationAvailabilityChecker
    {
        /// <summary>
        /// True if a non-cancelled reservation overlaps [checkIn, checkOut) for this room.
        /// Check-out day is exclusive: leaving on the 5th does not block arriving on the 5th.
        /// </summary>
        Task<bool> IsRoomBookedAsync(int roomId, DateTime checkIn, DateTime checkOut,
            CancellationToken c = default);
    }
}

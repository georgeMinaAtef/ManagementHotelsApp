using HotelApp.Domain.Entities.Rooms;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Domain.Entities.Reservations
{
    // Composite key: (ReservationId, RoomId)
    public class ReservationRoom
    {
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; } = null!;

        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;
    }

}

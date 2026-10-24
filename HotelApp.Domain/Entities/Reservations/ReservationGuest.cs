using HotelApp.Domain.Entities.Guests;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Domain.Entities.Reservations
{

    // Composite key: (ReservationId, GuestId)
    public class ReservationGuest
    {
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; } = null!;

        public int GuestId { get; set; }
        public Guest Guest { get; set; } = null!;
    }

}

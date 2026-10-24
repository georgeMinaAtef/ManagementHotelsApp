using HotelApp.Domain.Entities.Hotels;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Domain.Entities.Reservations
{
    // Composite key: (ServiceId, ReservationId)
    public class ReservationService
    {
        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;

        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; } = null!;
    }

}

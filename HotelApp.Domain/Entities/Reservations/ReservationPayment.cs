using HotelApp.Domain.Entities.Payments;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Domain.Entities.Reservations
{

    // Composite key: (ReservationId, PaymentId)
    public class ReservationPayment
    {
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; } = null!;

        public int PaymentId { get; set; }
        public Payment Payment { get; set; } = null!;
    }

}

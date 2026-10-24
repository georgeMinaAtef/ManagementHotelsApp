using HotelApp.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Domain.Entities.Reservations
{
    public class Reservation : BaseEntity<int>
    {
        public DateTime BookingDate { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string ReservationStatus { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }

        public ICollection<ReservationRoom> ReservationRooms { get; set; } = new List<ReservationRoom>();
        public ICollection<ReservationGuest> ReservationGuests { get; set; } = new List<ReservationGuest>();
        public ICollection<ReservationService> ReservationServices { get; set; } = new List<ReservationService>();
        public ICollection<ReservationPayment> ReservationPayments { get; set; } = new List<ReservationPayment>();
    }

}

using HotelApp.Domain.Common;
using HotelApp.Domain.Entities.Reservations;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Domain.Entities.Payments
{

    public class Payment : BaseEntity<int>
    {
        public string Method { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string ConfirmationNumber { get; set; } = string.Empty;

        public ICollection<ReservationPayment> ReservationPayments { get; set; } = new List<ReservationPayment>();
    }

}

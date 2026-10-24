using HotelApp.Domain.Common;
using HotelApp.Domain.Entities.Reservations;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Domain.Entities.Guests
{
    public class Guest : BaseEntity<int>
    {
        public string FullName { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public string PassportNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }

        public ICollection<GuestContactDetail> ContactDetails { get; set; } = new List<GuestContactDetail>();
        public ICollection<ReservationGuest> ReservationGuests { get; set; } = new List<ReservationGuest>();
    }

}

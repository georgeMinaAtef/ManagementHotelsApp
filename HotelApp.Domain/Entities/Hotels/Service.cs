using HotelApp.Domain.Common;
using HotelApp.Domain.Entities.Reservations;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Domain.Entities.Hotels
{
    public class Service : BaseEntity<int>
    {
        public string ServiceName { get; set; } = string.Empty;
        public decimal Charge { get; set; }
        public DateTime RequestDate { get; set; }

        public int StaffId { get; set; }
        public Staff Staff { get; set; } = null!;

        public ICollection<ReservationService> ReservationServices { get; set; } = new List<ReservationService>();
    }

}

using HotelApp.Domain.Common;
using HotelApp.Domain.Entities.Rooms;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Domain.Entities.Hotels
{
    public class Hotel : BaseEntity<int>
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int StarRating { get; set; }
        public string ContactNumber { get; set; } = string.Empty;

        // FK [Unique] - the staff member who manages this hotel
        public int? ManagerId { get; set; }
        public Staff Manager { get; set; } = null!;

        // Belongs To / Work At
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
        public ICollection<Staff> Staff { get; set; } = new List<Staff>();
    }

}

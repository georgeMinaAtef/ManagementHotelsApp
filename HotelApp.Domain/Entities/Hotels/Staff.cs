using HotelApp.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Domain.Entities.Hotels
{
    public class Staff : BaseEntity<int>
    {
        public string FullName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public decimal Salary { get; set; }

        public int HotelId { get; set; }
        public Hotel Hotel { get; set; } = null!;

        // Hotel managed by this staff member (if any)
        public Hotel? ManagedHotel { get; set; }

        // Provided By
        public ICollection<Service> Services { get; set; } = new List<Service>();
    }
}
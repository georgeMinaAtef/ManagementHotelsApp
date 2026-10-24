using HotelApp.Domain.Common;
using HotelApp.Domain.Entities.Hotels;
using HotelApp.Domain.Entities.Reservations;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Domain.Entities.Rooms
{
    public class Room : BaseEntity<int>
    {
        // Business identifier shown in the diagram as RoomNumber (keep unique per hotel)
        public string RoomNumber { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public decimal DailyRate { get; set; }
        public bool Availability { get; set; }

        public int HotelId { get; set; }
        public Hotel Hotel { get; set; } = null!;

        public ICollection<Amenity> Amenities { get; set; } = new List<Amenity>();
        public ICollection<ReservationRoom> ReservationRooms { get; set; } = new List<ReservationRoom>();
    }

}

using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Domain.Entities.Rooms
{
    // Composite key: (RoomId, Name)
    public class Amenity
    {
        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;

        public string Name { get; set; } = string.Empty;
    }

}

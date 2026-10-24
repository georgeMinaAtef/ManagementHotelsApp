using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Domain.Entities.Guests
{

    // Composite key: (GuestId, Detail)
    public class GuestContactDetail
    {
        public int GuestId { get; set; }
        public Guest Guest { get; set; } = null!;

        public string Detail { get; set; } = string.Empty;
    }


}
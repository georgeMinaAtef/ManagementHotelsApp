using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.DTOs.Guest
{
    public record GuestParams
    {
        public string? Search { get; set; }        // name or passport
        public string? Nationality { get; set; }
        public string? Sort { get; set; }          // nameAsc | nameDesc | dobAsc | dobDesc
    }
}

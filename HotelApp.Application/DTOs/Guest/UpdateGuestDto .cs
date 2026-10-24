using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.DTOs.Guest
{
    public record UpdateGuestDto
    {
        public int? GuestId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PassportNumber { get; set; } = string.Empty;
        public string? Nationality { get; set; }
        public DateOnly DateOfBirth { get; set; }   // لو النوع عندك DateTime غيّره هنا
        public List<string> ContactDetails { get; set; } = [];
    }

}

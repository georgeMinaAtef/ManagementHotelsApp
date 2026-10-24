using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.DTOs.Guest
{
    public record GuestDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public string PassportNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public List<string> ContactDetails { get; set; } = new();
    }

}

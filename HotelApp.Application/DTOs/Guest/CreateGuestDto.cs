using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HotelApp.Application.DTOs.Guest
{
    public record CreateGuestDto
    {
        [Required, MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(60)]
        public string Nationality { get; set; } = string.Empty;

        [Required, MaxLength(30)]
        public string PassportNumber { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        public List<string> ContactDetails { get; set; } = new();
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HotelApp.Application.DTOs.Rooms
{

    // Update request (HotelId comes from the route; a room cannot move to another hotel)
    public record UpdateRoomDto
    {
        [Required(ErrorMessage = "RoomNumber is required.")]
        [StringLength(10, ErrorMessage = "RoomNumber cannot exceed 10 characters.")]
        public string RoomNumber { get; init; } = null!;

        [Required(ErrorMessage = "RoomType is required.")]
        [StringLength(50, ErrorMessage = "RoomType cannot exceed 50 characters.")]
        public string RoomType { get; init; } = null!;

        [Range(1, 10, ErrorMessage = "Capacity must be between 1 and 10.")]
        public int Capacity { get; init; }

        [Range(1, 100000, ErrorMessage = "DailyRate must be between 1 and 100000.")]
        public decimal DailyRate { get; init; }

        public bool Availability { get; init; }
    }
}

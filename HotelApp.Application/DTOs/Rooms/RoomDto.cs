using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.DTOs.Rooms
{
    // Response
    public record RoomDto
    {
        public int RoomId { get; init; }
        public int HotelId { get; init; }
        public string RoomNumber { get; init; } = null!;
        public string RoomType { get; init; } = null!;
        public int Capacity { get; init; }
        public decimal DailyRate { get; init; }
        public bool Availability { get; init; }
    }
}

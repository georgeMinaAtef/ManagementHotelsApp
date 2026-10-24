using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HotelApp.Application.DTOs.Hotel
{
    public record HotelRoomSummaryDto(
        [property: Range(1, int.MaxValue, ErrorMessage = "RoomId must be a positive number.")] int RoomId,
        [property: Required(ErrorMessage = "RoomNumber is required.")]
        [property: StringLength(20, ErrorMessage = "RoomNumber cannot exceed 20 characters.")] string RoomNumber,
        [property: Required(ErrorMessage = "RoomType is required.")]
        [property: StringLength(50, ErrorMessage = "RoomType cannot exceed 50 characters.")] string RoomType,
        [property: Range(1, 20, ErrorMessage = "Capacity must be between 1 and 20.")] int Capacity,
        [property: Range(typeof(decimal), "0.01", "1000000", ErrorMessage = "DailyRate must be between 0.01 and 1,000,000.")] decimal DailyRate,
        bool Availability);
}


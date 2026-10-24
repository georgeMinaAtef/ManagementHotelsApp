using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HotelApp.Application.DTOs.Hotel
{

    public record HotelDetailsDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "HotelId must be a positive number.")]
        public int HotelId { get; init; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150, ErrorMessage = "Name cannot exceed 150 characters.")]
        public string Name { get; init; } = null!;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
        public string Address { get; init; } = null!;

        [Required(ErrorMessage = "City is required.")]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters.")]
        public string City { get; init; } = null!;

        [Range(1, 5, ErrorMessage = "StarRating must be between 1 and 5.")]
        public int StarRating { get; init; }

        [Required(ErrorMessage = "ContactNumber is required.")]
        [StringLength(30, ErrorMessage = "ContactNumber cannot exceed 30 characters.")]
        public string ContactNumber { get; init; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "ManagerId must be a positive number.")]
        public int? ManagerId { get; init; }

        [StringLength(200, ErrorMessage = "ManagerName cannot exceed 200 characters.")]
        public string? ManagerName { get; init; }

        [Range(0, int.MaxValue, ErrorMessage = "RoomsCount cannot be negative.")]
        public int RoomsCount { get; init; }

        [Range(0, int.MaxValue, ErrorMessage = "StaffCount cannot be negative.")]
        public int StaffCount { get; init; }

        [Required(ErrorMessage = "Rooms list is required.")]
        public List<HotelRoomSummaryDto> Rooms { get; init; } = new();

        [Required(ErrorMessage = "Staff list is required.")]
        public List<HotelStaffSummaryDto> Staff { get; init; } = new();
    }

}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HotelApp.Application.DTOs.Staff
{
    public abstract record StaffWriteDto
    {
        [Required(ErrorMessage = "FullName is required.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "FullName must be between 3 and 200 characters.")]
        public string FullName { get; init; } = null!;

        [Required(ErrorMessage = "Position is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Position must be between 2 and 100 characters.")]
        public string Position { get; init; } = null!;

        [Range(typeof(decimal), "0", "10000000", ErrorMessage = "Salary must be between 0 and 10,000,000.")]
        public decimal Salary { get; init; }

        [Range(1, int.MaxValue, ErrorMessage = "HotelId must be a positive number.")]
        public int HotelId { get; init; }
    }

    public record CreateStaffDto : StaffWriteDto;
    public record UpdateStaffDto : StaffWriteDto;

    public record StaffDto  : StaffWriteDto
    {
        public int StaffId { get; init; }
        public bool IsManager { get; init; }
    }
}

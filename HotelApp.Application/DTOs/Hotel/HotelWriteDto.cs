using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HotelApp.Application.DTOs.Hotel
{
    public abstract record HotelWriteDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 150 characters.")]
        public string Name { get; init; } = null!;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(250, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 250 characters.")]
        public string Address { get; init; } = null!;

        [Required(ErrorMessage = "City is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "City must be between 2 and 100 characters.")]
        public string City { get; init; } = null!;

        [Range(1, 5, ErrorMessage = "StarRating must be between 1 and 5.")]
        public int StarRating { get; init; }

        [Required(ErrorMessage = "ContactNumber is required.")]
        [StringLength(30, ErrorMessage = "ContactNumber cannot exceed 30 characters.")]
        [RegularExpression(@"^\+?[0-9\s\-()]{7,30}$",
            ErrorMessage = "ContactNumber must be a valid phone number.")]
        public string ContactNumber { get; init; } = null!;

        /// <summary>Optional. Must be the Id of an existing Staff member.</summary>
        [Range(1, int.MaxValue, ErrorMessage = "ManagerId must be a positive number.")]
        public int? ManagerId { get; init; }
    }

    public record CreateHotelDto : HotelWriteDto;

    public record UpdateHotelDto : HotelWriteDto;
    //{
    //    [Range(1, int.MaxValue, ErrorMessage = "HotelId must be a positive number.")]
    //    public int HotelId { get; init; }
    //}
}

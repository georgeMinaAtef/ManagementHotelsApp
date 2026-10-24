using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HotelApp.Application.DTOs.Hotel
{
    public record HotelStaffSummaryDto(
        [property: Range(1, int.MaxValue, ErrorMessage = "StaffId must be a positive number.")] int StaffId,
        [property: Required(ErrorMessage = "FullName is required.")]
        [property: StringLength(200, ErrorMessage = "FullName cannot exceed 200 characters.")] string FullName,
        [property: Required(ErrorMessage = "Position is required.")]
        [property: StringLength(100, ErrorMessage = "Position cannot exceed 100 characters.")] string Position);

}

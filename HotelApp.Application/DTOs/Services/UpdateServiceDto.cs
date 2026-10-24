namespace HotelApp.Application.DTOs.Services
{
    // الشكل اللي بيدخل عند التعديل
    public record UpdateServiceDto(
        string ServiceName,
        decimal Charge,
        DateTime RequestDate,
        int StaffId);
}

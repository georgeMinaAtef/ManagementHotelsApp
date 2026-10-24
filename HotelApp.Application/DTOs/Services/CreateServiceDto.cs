namespace HotelApp.Application.DTOs.Services
{
    // الشكل اللي بيدخل عند الإضافة
    public record CreateServiceDto(
        string ServiceName,
        decimal Charge,
        DateTime RequestDate,
        int StaffId);
}

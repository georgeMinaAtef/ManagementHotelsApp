namespace HotelApp.Application.DTOs.Services
{
    // الشكل اللي بيرجع من الـ API
    public record ServiceDto(
        int Id,
        string ServiceName,
        decimal Charge,
        DateTime RequestDate,
        int StaffId);
}

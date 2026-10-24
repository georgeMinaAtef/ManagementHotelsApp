namespace HotelApp.Application.DTOs.Reservations
{
    public class ReservedServiceDto
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public decimal Charge { get; set; }
    }
}

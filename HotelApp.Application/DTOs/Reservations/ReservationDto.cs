using HotelApp.Application.DTOs.Guest;

namespace HotelApp.Application.DTOs.Reservations
{
    public class ReservationDto
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }

        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;

        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }
        public int Nights { get; set; }

        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }
        public decimal TotalPrice { get; set; }

        public List<GuestDto> Guests { get; set; } = [];
        public List<ReservedServiceDto> Services { get; set; } = [];
    }
}

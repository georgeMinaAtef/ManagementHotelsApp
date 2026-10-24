using HotelApp.Application.DTOs.Guest;

namespace HotelApp.Application.DTOs.Reservations
{
    // تأكيد الحجز: بعد ما تعمل Hold وتاخد الـ HoldToken
    public class CreateReservationDto
    {
        public int RoomId { get; set; }
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }
        public List<CreateGuestDto> Guests { get; set; } = new();

    }
}

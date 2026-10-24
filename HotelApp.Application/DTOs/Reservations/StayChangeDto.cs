namespace HotelApp.Application.DTOs.Reservations
{
    // تغيير الإقامة (اختياري). أي قيمة مش مبعوتة بتفضل زي ما هي.
    public class StayChangeDto
    {
        public int? RoomId { get; set; }
        public DateOnly? CheckIn { get; set; }
        public DateOnly? CheckOut { get; set; }
    }
}

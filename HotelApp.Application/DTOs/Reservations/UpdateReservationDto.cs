using HotelApp.Application.DTOs.Guest;

namespace HotelApp.Application.DTOs.Reservations
{
    // تعديل الحجز: عدد الأشخاص + قائمة الأشخاص الكاملة + تغيير الإقامة (اختياري)
    public class UpdateReservationDto
    {
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }

        // القائمة دي هي الحالة النهائية للحجز:
        // اللي له GuestId بيتعدل، واللي من غير GuestId بيتضاف، واللي مش موجود في القائمة بيتشال من الحجز
        public List<UpdateGuestDto> Guests { get; set; } = [];

        // null = مفيش تغيير في الغرفة أو التواريخ
        public StayChangeDto? Stay { get; set; }
    }
}

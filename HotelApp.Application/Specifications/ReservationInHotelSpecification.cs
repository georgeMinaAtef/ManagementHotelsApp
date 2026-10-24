using HotelApp.Application.Common;
using HotelApp.Domain.Entities.Guests;
using HotelApp.Domain.Entities.Hotels;
using HotelApp.Domain.Entities.Reservations;

namespace HotelApp.Application.Specifications
{
    // 1) حجز بالـ Id بشرط يكون تابع للفندق (عن طريق غرفته) + تحميل الغرف والأشخاص والخدمات
    //    (Tracking شغال لأننا بنعدل عليه)
    public class ReservationInHotelSpecification : BaseSpecification<Reservation, int>
    {
        public ReservationInHotelSpecification(int id, int hotelId)
            : base(r => r.Id == id && r.ReservationRooms.Any(rr => rr.Room.HotelId == hotelId))
        {
            AddInclude(r => r.ReservationRooms);
            AddInclude(r => r.ReservationGuests);
            AddInclude(r => r.ReservationServices);
            EnableSplitQuery();
        }
    }

    // 2) في حجز تاني (مش ملغي) بيتقاطع مع نفس الغرفة والتواريخ؟
    //    excludeReservationId = الحجز اللي بنعدله (عشان ما يتعارضش مع نفسه)
    public class OverlappingReservationSpecification : BaseSpecification<Reservation, int>
    {
        public OverlappingReservationSpecification(int roomId, DateTime checkIn, DateTime checkOut, int excludeReservationId)
            : base(r => r.Id != excludeReservationId
                        && r.ReservationStatus != ReservationStatuses.Cancelled
                        && r.CheckInDate < checkOut
                        && r.CheckOutDate > checkIn
                        && r.ReservationRooms.Any(rr => rr.RoomId == roomId))
        {
            EnableNoTracking();
        }
    }

    // 3) أشخاص بالـ Ids (مع بيانات التواصل)
    public class GuestsByIdsSpecification : BaseSpecification<Guest, int>
    {
        public GuestsByIdsSpecification(int[] ids) : base(g => ids.Contains(g.Id))
        {
            AddInclude(g => g.ContactDetails);
        }
    }

    // 4) أشخاص موجودين قبل كده بأرقام الباسبور (عشان ما نكررش نفس الشخص)
    public class GuestsByPassportsSpecification : BaseSpecification<Guest, int>
    {
        public GuestsByPassportsSpecification(string[] passports)
            : base(g => passports.Contains(g.PassportNumber))
        {
            AddInclude(g => g.ContactDetails);
        }
    }

    // 5) خدمات بالـ Ids (قراءة فقط: للأسماء والأسعار)
    public class ServicesByIdsSpecification : BaseSpecification<Service, int>
    {
        public ServicesByIdsSpecification(int[] ids) : base(s => ids.Contains(s.Id))
        {
            EnableNoTracking();
        }
    }
}

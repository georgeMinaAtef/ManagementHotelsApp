using AutoMapper;
using HotelApp.Application.DTOs.Guest;
using HotelApp.Domain.Entities.Guests;

namespace HotelApp.Application.Mapping
{
    // لو عندك Profile للحجوزات بالفعل، انقل الـ CreateMap جواه واحذف الكلاس ده
    public class ReservationProfile : Profile
    {
        public ReservationProfile()
        {
            // بيستخدم للإنشاء وللتعديل (Map(dto, existingGuest))
            // ContactDetails بنتعامل معاها يدوي في الـ service عشان نعمل مقارنة (إضافة/حذف) مش استبدال
            CreateMap<UpdateGuestDto, Guest>()
                .ForMember(d => d.FullName, o => o.MapFrom(s => s.FullName.Trim()))
                .ForMember(d => d.PassportNumber, o => o.MapFrom(s => s.PassportNumber.Trim()))
                .ForMember(d => d.Nationality, o => o.MapFrom(s => (s.Nationality ?? "").Trim()))
                .ForMember(d => d.DateOfBirth, o => o.MapFrom(s => s.DateOfBirth))
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.ContactDetails, o => o.Ignore());
        }
    }
}
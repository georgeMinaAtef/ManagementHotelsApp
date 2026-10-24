using AutoMapper;
using HotelApp.Application.DTOs.Hotel;
using HotelApp.Domain.Entities.Hotels;
using HotelApp.Domain.Entities.Rooms;

namespace HotelApp.Application.Profiles
{
    public class HotelProfile : Profile
    {
        public HotelProfile()
        {
            // Entity -> response DTOs
            CreateMap<Hotel, HotelDto>()
                .ForMember(d => d.HotelId, o => o.MapFrom(s => s.Id));

            CreateMap<Hotel, HotelDetailsDto>()
                .ForMember(d => d.HotelId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.ManagerName, o => o.MapFrom(s => s.Manager != null ? s.Manager.FullName : null))
                .ForMember(d => d.RoomsCount, o => o.MapFrom(s => s.Rooms.Count))
                .ForMember(d => d.StaffCount, o => o.MapFrom(s => s.Staff.Count));

            CreateMap<Room, HotelRoomSummaryDto>()
                .ForCtorParam("RoomId", o => o.MapFrom(s => s.Id));

            CreateMap<Staff, HotelStaffSummaryDto>()
                .ForCtorParam("StaffId", o => o.MapFrom(s => s.Id));

            // Request DTOs -> entity
            CreateMap<CreateHotelDto, Hotel>();

            CreateMap<UpdateHotelDto, Hotel>()
                .ForMember(d => d.Id, o => o.Ignore());
        }
    }
}
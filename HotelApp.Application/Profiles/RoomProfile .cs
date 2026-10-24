using AutoMapper;
using HotelApp.Application.DTOs.Rooms;
using HotelApp.Domain.Entities.Rooms;

namespace HotelApp.Application.Mappings
{
    public class RoomProfile : Profile
    {
        public RoomProfile()
        {
            CreateMap<Room, RoomDto>()
                .ForMember(d => d.RoomId, o => o.MapFrom(s => s.Id));

            // HotelId is set by RoomService from the route
            CreateMap<CreateRoomDto, Room>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.HotelId, o => o.Ignore())
                .ForMember(d => d.Hotel, o => o.Ignore())
                .ForMember(d => d.Amenities, o => o.Ignore())
                .ForMember(d => d.ReservationRooms, o => o.Ignore());

            CreateMap<UpdateRoomDto, Room>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.HotelId, o => o.Ignore())
                .ForMember(d => d.Hotel, o => o.Ignore())
                .ForMember(d => d.Amenities, o => o.Ignore())
                .ForMember(d => d.ReservationRooms, o => o.Ignore());
        }
    }
}
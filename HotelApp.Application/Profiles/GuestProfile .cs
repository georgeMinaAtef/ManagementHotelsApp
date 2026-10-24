using AutoMapper;
using HotelApp.Application.DTOs.Guest;
using HotelApp.Domain.Entities.Guests;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.Profiles
{
    public class GuestProfile : Profile
    {
        public GuestProfile()
        {
            CreateMap<Guest, GuestDto>()
                .ForMember(d => d.ContactDetails,
                           o => o.MapFrom(s => s.ContactDetails.Select(c => c.Detail)));

            // ContactDetails are built manually in GuestService (composite key)
            CreateMap<CreateGuestDto, Guest>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.ContactDetails, o => o.Ignore())
                .ForMember(d => d.ReservationGuests, o => o.Ignore());
        }

    }
}

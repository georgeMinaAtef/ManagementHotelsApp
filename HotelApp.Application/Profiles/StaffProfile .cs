using AutoMapper;
using HotelApp.Application.DTOs.Staff;
using HotelApp.Domain.Entities.Hotels;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.Profiles
{
    public class StaffProfile : Profile
    {
        public StaffProfile()
        {
            CreateMap<Staff, StaffDto>()
                .ForMember(d => d.StaffId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.IsManager, o => o.MapFrom(s => s.ManagedHotel != null));

            CreateMap<CreateStaffDto, Staff>();

            CreateMap<UpdateStaffDto, Staff>()
                .ForMember(d => d.Id, o => o.Ignore());
        }
    }
}

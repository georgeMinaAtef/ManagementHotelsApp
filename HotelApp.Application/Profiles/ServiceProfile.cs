using AutoMapper;
using HotelApp.Application.DTOs.Services;
using HotelApp.Domain.Entities.Hotels;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.Profiles
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {
            // Entity -> DTO (للعرض)
            CreateMap<Service, ServiceDto>();

            // DTO -> Entity (للإضافة)
            CreateMap<CreateServiceDto, Service>()
                .ForMember(d => d.ServiceName, o => o.MapFrom(s => s.ServiceName.Trim()))
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.Staff, o => o.Ignore())
                .ForMember(d => d.ReservationServices, o => o.Ignore());

            // DTO -> Entity الموجود (للتعديل)
            CreateMap<UpdateServiceDto, Service>()
                .ForMember(d => d.ServiceName, o => o.MapFrom(s => s.ServiceName.Trim()))
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.Staff, o => o.Ignore())
                .ForMember(d => d.ReservationServices, o => o.Ignore());
        }
    }
}

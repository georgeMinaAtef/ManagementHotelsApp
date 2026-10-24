using HotelApp.Application.Contracts;
using HotelApp.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HotelApp.Application
{
    public static class ApplicationServicesRegistrations
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(c => { }, typeof(ApplicationServicesRegistrations).Assembly);
            services.AddScoped<IAuthenticationsService, AuthenticationsService>();
            services.AddScoped<IHotelService, HotelService>();
            services.AddScoped<IStaffService, StaffService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IGuestService, GuestService>();
            services.AddScoped<IServiceService, ServiceService>();
            services.AddScoped<IReservationBookingService, ReservationBookingService>();
            return services;
        }
    }
}
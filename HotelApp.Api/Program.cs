using HotelApp.Application;
using HotelApp.Application.Contracts;
using HotelApp.Application.Services;
using HotelApp.Infrastructure;
using HotelApp.Infrastructure.Identity.Service;
using Scalar.AspNetCore;
using HotelApp.Api.OpenApi;

namespace HotelApp.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // 1- Infrastructure الاول
            builder.Services.AddInfrastructureServices(builder.Configuration);
            // 2- Application
            builder.Services.AddApplicationServices();

            // 3- تسجيل اجباري مباشر - Double guarantee
            // حتى لو الـ extension فشل، دول هيتسجلوا
            builder.Services.AddScoped<IReservationBookingService, ReservationBookingService>();

            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));
            builder.Services.AddAuthorization();
            builder.Services.AddOpenApi(o => o.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

            var app = builder.Build();

            app.MapOpenApi();
            app.MapScalarApiReference(o => {
                o.Title = "HotelApp API v1";
                o.AddPreferredSecuritySchemes("Bearer");
            });

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}


// to show ui in chrome https://localhost:7195/scalar/v1

// ai https://claude.ai/new?redirect=claude.com&via=static_google_success


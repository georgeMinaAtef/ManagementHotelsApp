using HotelApp.Application.Contracts;
using HotelApp.Domain.Contracts;
using HotelApp.Infrastructure.Identity.Data;
using HotelApp.Infrastructure.Identity.Entities;
using HotelApp.Infrastructure.Identity.Service;
using HotelApp.Infrastructure.Repositories;
using HotelManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

namespace HotelApp.Infrastructure
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<HotelDbContext>(o => o.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<HotelIdentityDbContext>(o => o.UseSqlServer(configuration.GetConnectionString("IdentityConnection")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IReservationAvailabilityChecker, ReservationAvailabilityChecker>();

            // Redis + Cache Implementation
            var redisConn = configuration.GetConnectionString("Redis") ?? "localhost:6379";
            services.AddSingleton<IConnectionMultiplexer>(_ => {
                var opt = ConfigurationOptions.Parse(redisConn);
                opt.AbortOnConnectFail = false;
                return ConnectionMultiplexer.Connect(opt);
            });

            // ده الصح - الـ Interface في Domain والـ Implementation في Infrastructure
            services.AddScoped<IReservationRoomCache, RedisReservationRoomCache>();

            services.AddScoped<IIdentitytService, IdentitytService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddIdentityCore<ApplicationUser>().AddRoles<IdentityRole>().AddEntityFrameworkStores<HotelIdentityDbContext>();

            var jwt = configuration.GetSection("JWT").Get<JwtSettings>()!;
            services.AddAuthentication(o => {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o => {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey))
                };
            });

            services.AddScoped<IReservationRoomCache, RedisReservationRoomCache>();



            return services;
        }
    }
}
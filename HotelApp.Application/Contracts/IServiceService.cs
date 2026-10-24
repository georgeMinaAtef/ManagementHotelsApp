using HotelApp.Application.Common;
using HotelApp.Application.DTOs.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.Contracts
{
    public interface IServiceService
    {
        Task<Result<IReadOnlyList<ServiceDto>>> GetAllAsync(int hotelId, string? search = null);
        Task<Result<ServiceDto>> GetByIdAsync(int hotelId, int id);
        Task<Result<ServiceDto>> CreateAsync(int hotelId, CreateServiceDto dto);
        Task<Result> UpdateAsync(int hotelId, int id, UpdateServiceDto dto);
        Task<Result> DeleteAsync(int hotelId, int id);

    }

}

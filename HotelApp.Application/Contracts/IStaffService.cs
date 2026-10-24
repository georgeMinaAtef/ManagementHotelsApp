using HotelApp.Application.Common;
using HotelApp.Application.DTOs.Staff;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.Contracts
{
    public interface IStaffService
    {
        Task<Result<IReadOnlyList<StaffDto>>> GetAllAsync(int? hotelId = null, CancellationToken ct = default);
        Task<Result<StaffDto>> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Result<StaffDto>> CreateAsync(CreateStaffDto dto, CancellationToken ct = default);
        Task<Result<StaffDto>> UpdateAsync(int id, UpdateStaffDto dto, CancellationToken ct = default);
        Task<Result> DeleteAsync(int id, CancellationToken ct = default);
    }
}

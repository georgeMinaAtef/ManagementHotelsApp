using HotelApp.Application.Common;
using HotelApp.Application.DTOs.Guest;

namespace HotelApp.Application.Contracts
{
    public interface IGuestService
    {
        Task<Result<IReadOnlyList<GuestDto>>> GetAllAsync(int hotelId, GuestParams p, CancellationToken c = default);
        Task<Result<GuestDto>> GetByIdAsync(int hotelId, int id, CancellationToken c = default);
        Task<Result<GuestDto>> CreateAsync(int hotelId, CreateGuestDto dto, CancellationToken c = default);
        Task<Result<GuestDto>> UpdateAsync(int hotelId, int id, UpdateGuestDto dto, CancellationToken c = default);
        Task<Result> DeleteAsync(int hotelId, int id, CancellationToken c = default);

    }
}
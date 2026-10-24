using HotelApp.Application.Common;
using HotelApp.Application.DTOs.Hotel;

namespace HotelApp.Application.Contracts
{
    public interface IHotelService
    {
        Task<Result<IReadOnlyList<HotelDto>>> GetAllHotelsAsync(CancellationToken cancellationToken = default);
        Task<Result<HotelDetailsDto>> GetHotelByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Result<HotelDto>> CreateHotelAsync(CreateHotelDto dto, CancellationToken cancellationToken = default);
        Task<Result<HotelDto>> UpdateHotelAsync(int id, UpdateHotelDto dto, CancellationToken cancellationToken = default);
        Task<Result> DeleteHotelAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> HotelExistsAsync(int id, CancellationToken cancellationToken = default);

        // Manager operations
        Task<Result<HotelDto>> AddManagerAsync(int hotelId, int managerId, CancellationToken cancellationToken = default);
        Task<Result<HotelDto>> UpdateManagerAsync(int hotelId, int newManagerId, CancellationToken cancellationToken = default);
        Task<Result> RemoveManagerAsync(int hotelId, CancellationToken cancellationToken = default);
    }
}
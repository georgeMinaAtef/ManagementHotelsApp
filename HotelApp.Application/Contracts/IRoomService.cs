using HotelApp.Application.Common;
using HotelApp.Application.DTOs.Rooms;

namespace HotelApp.Application.Contracts
{
    public interface IRoomService
    {
        Task<Result<IReadOnlyList<RoomDto>>> GetRoomsByHotelAsync(int hotelId, CancellationToken ct = default);
        Task<Result<RoomDto>> GetRoomByIdAsync(int hotelId, int id, CancellationToken ct = default);
        Task<Result<RoomDto>> CreateRoomAsync(int hotelId, CreateRoomDto dto, CancellationToken ct = default);
        Task<Result<RoomDto>> UpdateRoomAsync(int hotelId, int id, UpdateRoomDto dto, CancellationToken ct = default);
        Task<Result> DeleteRoomAsync(int hotelId, int id, CancellationToken ct = default);

    }
}
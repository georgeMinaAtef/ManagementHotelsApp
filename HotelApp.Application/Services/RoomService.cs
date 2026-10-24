using AutoMapper;
using HotelApp.Application.Common;
using HotelApp.Application.Contracts;
using HotelApp.Application.DTOs.Rooms;
using HotelApp.Application.Specifications;
using HotelApp.Domain.Contracts;
using HotelApp.Domain.Entities.Hotels;
using HotelApp.Domain.Entities.Rooms;

namespace HotelApp.Application.Services
{
    public class RoomService(IUnitOfWork unitOfWork, IMapper mapper) : IRoomService
    {
        private IGenericRepository<Room, int> Repo => unitOfWork.GetRepository<Room, int>();
        private IGenericRepository<Hotel, int> HotelRepo => unitOfWork.GetRepository<Hotel, int>();

        private static Error HotelNotFound(int hotelId)
            => Error.NotFound("Room.HotelNotFound", $"Hotel with id {hotelId} was not found.");

        private static Error RoomNotFound(int hotelId, int id)
            => Error.NotFound("Room.NotFound", $"Room {id} was not found in hotel {hotelId}.");

        private static Error DuplicateNumber(string number)
            => Error.Conflict("Room.DuplicateNumber", $"Room number {number} already exists in this hotel.");

        private static Error HasReservations(int id)
            => Error.Conflict("Room.HasReservations", $"Room {id} has reservations and cannot be deleted.");

        public async Task<Result<IReadOnlyList<RoomDto>>> GetRoomsByHotelAsync(int hotelId, CancellationToken ct = default)
        {
            if (await HotelRepo.GetByIdAsync(hotelId, ct) is null)
                return Result<IReadOnlyList<RoomDto>>.Fail(HotelNotFound(hotelId));

            var rooms = await Repo.GetAllAsync(new RoomSpecification(hotelId, false), ct);
            return Result<IReadOnlyList<RoomDto>>.Ok(mapper.Map<IReadOnlyList<RoomDto>>(rooms));
        }

        public async Task<Result<RoomDto>> GetRoomByIdAsync(int hotelId, int id, CancellationToken ct = default)
        {
            var room = await Repo.GetAsync(RoomSpecification.ByHotelAndId(hotelId, id), ct);
            if (room is null)
                return Result<RoomDto>.Fail(RoomNotFound(hotelId, id));

            return Result<RoomDto>.Ok(mapper.Map<RoomDto>(room));
        }

        public async Task<Result<RoomDto>> CreateRoomAsync(int hotelId, CreateRoomDto dto, CancellationToken ct = default)
        {
            // 1) Hotel must exist
            if (await HotelRepo.GetByIdAsync(hotelId, ct) is null)
                return Result<RoomDto>.Fail(HotelNotFound(hotelId));

            // 2) Room number must be unique inside the hotel
            var existing = await Repo.GetAsync(new RoomSpecification(hotelId, dto.RoomNumber), ct);
            if (existing is not null)
                return Result<RoomDto>.Fail(DuplicateNumber(dto.RoomNumber));

            // 3) Save (HotelId comes from the route)
            var room = mapper.Map<Room>(dto);
            room.HotelId = hotelId;

            Repo.Add(room, ct);
            await unitOfWork.SaveChanges(ct);

            return Result<RoomDto>.Ok(mapper.Map<RoomDto>(room));
        }

        public async Task<Result<RoomDto>> UpdateRoomAsync(int hotelId, int id, UpdateRoomDto dto, CancellationToken ct = default)
        {
            // Tracked room that must belong to this hotel
            var room = await Repo.GetAsync(RoomSpecification.ForUpdate(hotelId, id), ct);
            if (room is null)
                return Result<RoomDto>.Fail(RoomNotFound(hotelId, id));

            // If the number changed, make sure it is free inside the same hotel
            if (room.RoomNumber != dto.RoomNumber)
            {
                var duplicate = await Repo.GetAsync(new RoomSpecification(hotelId, dto.RoomNumber), ct);
                if (duplicate is not null)
                    return Result<RoomDto>.Fail(DuplicateNumber(dto.RoomNumber));
            }

            mapper.Map(dto, room);   // Id and HotelId are ignored in RoomProfile

            Repo.Update(room, ct);
            await unitOfWork.SaveChanges(ct);

            return Result<RoomDto>.Ok(mapper.Map<RoomDto>(room));
        }

        public async Task<Result> DeleteRoomAsync(int hotelId, int id, CancellationToken ct = default)
        {
            // Tracked room (belonging to this hotel) + its reservations in one query
            var room = await Repo.GetAsync(RoomSpecification.WithReservations(hotelId, id), ct);
            if (room is null)
                return Result.Fail(RoomNotFound(hotelId, id));

            if (room.ReservationRooms.Count > 0)
                return Result.Fail(HasReservations(id));

            Repo.Delete(room, ct);
            await unitOfWork.SaveChanges(ct);

            return Result.Ok();
        }
    }
}
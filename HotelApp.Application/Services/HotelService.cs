using AutoMapper;
using HotelApp.Application.Common;
using HotelApp.Application.Contracts;
using HotelApp.Application.DTOs.Hotel;
using HotelApp.Application.Specifications;
using HotelApp.Domain.Contracts;
using HotelApp.Domain.Entities.Hotels;

namespace HotelApp.Application.Services
{
    public class HotelService(IUnitOfWork unitOfWork, IMapper mapper) : IHotelService
    {
        private IGenericRepository<Hotel, int> Repo => unitOfWork.GetRepository<Hotel, int>();
        private IGenericRepository<Staff, int> StaffRepo => unitOfWork.GetRepository<Staff, int>();

        private static Error HotelNotFound(int id)
            => Error.NotFound("Hotel.NotFound", $"Hotel with id {id} was not found.");

        private static Error ManagerNotFound(int id)
            => Error.NotFound("Hotel.ManagerNotFound", $"Manager (staff) with id {id} was not found.");

        private static Error ManagerAlreadyAssigned(int id)
            => Error.Conflict("Hotel.ManagerAlreadyAssigned",
                $"Staff member {id} already manages another hotel.");
        private static Error HotelAlreadyHasManager(int hotelId)
    => Error.Conflict("Hotel.AlreadyHasManager",
        $"Hotel {hotelId} already has a manager. Use update instead.");

        private static Error HotelHasNoManager(int hotelId)
            => Error.Conflict("Hotel.HasNoManager",
                $"Hotel {hotelId} does not have a manager.");

        private static Error SameManager(int managerId)
            => Error.Conflict("Hotel.SameManager",
                $"Staff member {managerId} is already the manager of this hotel.");

        // Returns null if the manager is valid (or not provided), otherwise an Error.
        // currentHotelId: the hotel being updated (null on create)
        private async Task<Error?> ValidateManagerAsync(int? managerId, int? currentHotelId, CancellationToken ct)
        {
            if (managerId is null)
                return null;

            var manager = await StaffRepo.GetAsync(StaffSpecification.ByIdReadOnly(managerId.Value), ct);
            if (manager is null)
                return ManagerNotFound(managerId.Value);

            // Manager is one-to-one: a staff member can manage only one hotel
            if (manager.ManagedHotel is not null && manager.ManagedHotel.Id != currentHotelId)
                return ManagerAlreadyAssigned(managerId.Value);

            return null;
        }

        public async Task<Result<IReadOnlyList<HotelDto>>> GetAllHotelsAsync(CancellationToken cancellationToken = default)
        {
            var hotels = await Repo.GetAllAsync(new HotelSpecification(), cancellationToken);
            return Result<IReadOnlyList<HotelDto>>.Ok(mapper.Map<IReadOnlyList<HotelDto>>(hotels));
        }

        public async Task<Result<HotelDetailsDto>> GetHotelByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var hotel = await Repo.GetAsync(new HotelSpecification(id), cancellationToken);
            if (hotel is null)
                return Result<HotelDetailsDto>.Fail(HotelNotFound(id));

            return Result<HotelDetailsDto>.Ok(mapper.Map<HotelDetailsDto>(hotel));
        }

        public async Task<Result<HotelDto>> CreateHotelAsync(CreateHotelDto dto, CancellationToken cancellationToken = default)
        {
            var managerError = await ValidateManagerAsync(dto.ManagerId, null, cancellationToken);
            if (managerError is not null)
                return Result<HotelDto>.Fail(managerError);

            var hotel = mapper.Map<Hotel>(dto);

            Repo.Add(hotel, cancellationToken);
            await unitOfWork.SaveChanges(cancellationToken);

            return Result<HotelDto>.Ok(mapper.Map<HotelDto>(hotel));
        }

        public async Task<Result<HotelDto>> UpdateHotelAsync(int id, UpdateHotelDto dto, CancellationToken cancellationToken = default)
        {
            var hotel = await Repo.GetByIdAsync(id, cancellationToken);   // tracked
            if (hotel is null)
                return Result<HotelDto>.Fail(HotelNotFound(id));

            var managerError = await ValidateManagerAsync(dto.ManagerId, id, cancellationToken);
            if (managerError is not null)
                return Result<HotelDto>.Fail(managerError);

            mapper.Map(dto, hotel);   // Id is ignored in HotelProfile

            Repo.Update(hotel, cancellationToken);
            await unitOfWork.SaveChanges(cancellationToken);

            return Result<HotelDto>.Ok(mapper.Map<HotelDto>(hotel));
        }

        public async Task<Result> DeleteHotelAsync(int id, CancellationToken cancellationToken = default)
        {
            // Read-only check first: relations are Restrict, so deleting a hotel
            // with children would throw an FK error (500) instead of a clean 409
            var details = await Repo.GetAsync(new HotelSpecification(id), cancellationToken);
            if (details is null)
                return Result.Fail(HotelNotFound(id));

            if (details.Rooms.Count > 0 || details.Staff.Count > 0)
                return Result.Fail(Error.Conflict("Hotel.HasChildren",
                    "Cannot delete a hotel that still has rooms or staff."));

            var hotel = await Repo.GetByIdAsync(id, cancellationToken);   // tracked
            Repo.Delete(hotel!, cancellationToken);
            await unitOfWork.SaveChanges(cancellationToken);

            return Result.Ok();
        }

        public async Task<bool> HotelExistsAsync(int id, CancellationToken cancellationToken = default)
            => await Repo.GetByIdAsync(id, cancellationToken) is not null;


        public async Task<Result<HotelDto>> AddManagerAsync(int hotelId, int managerId, CancellationToken cancellationToken = default)
        {
            var hotel = await Repo.GetByIdAsync(hotelId, cancellationToken);   // tracked
            if (hotel is null)
                return Result<HotelDto>.Fail(HotelNotFound(hotelId));

            if (hotel.ManagerId is not null)
                return Result<HotelDto>.Fail(HotelAlreadyHasManager(hotelId));

            var managerError = await ValidateManagerAsync(managerId, hotelId, cancellationToken);
            if (managerError is not null)
                return Result<HotelDto>.Fail(managerError);

            hotel.ManagerId = managerId;

            Repo.Update(hotel, cancellationToken);
            await unitOfWork.SaveChanges(cancellationToken);

            return Result<HotelDto>.Ok(mapper.Map<HotelDto>(hotel));
        }

        public async Task<Result<HotelDto>> UpdateManagerAsync(int hotelId, int newManagerId, CancellationToken cancellationToken = default)
        {
            var hotel = await Repo.GetByIdAsync(hotelId, cancellationToken);   // tracked
            if (hotel is null)
                return Result<HotelDto>.Fail(HotelNotFound(hotelId));

            if (hotel.ManagerId is null)
                return Result<HotelDto>.Fail(HotelHasNoManager(hotelId));

            if (hotel.ManagerId == newManagerId)
                return Result<HotelDto>.Fail(SameManager(newManagerId));

            var managerError = await ValidateManagerAsync(newManagerId, hotelId, cancellationToken);
            if (managerError is not null)
                return Result<HotelDto>.Fail(managerError);

            hotel.ManagerId = newManagerId;

            Repo.Update(hotel, cancellationToken);
            await unitOfWork.SaveChanges(cancellationToken);

            return Result<HotelDto>.Ok(mapper.Map<HotelDto>(hotel));
        }

        public async Task<Result> RemoveManagerAsync(int hotelId, CancellationToken cancellationToken = default)
        {
            var hotel = await Repo.GetByIdAsync(hotelId, cancellationToken);   // tracked
            if (hotel is null)
                return Result.Fail(HotelNotFound(hotelId));

            if (hotel.ManagerId is null)
                return Result.Fail(HotelHasNoManager(hotelId));

            hotel.ManagerId = null;

            Repo.Update(hotel, cancellationToken);
            await unitOfWork.SaveChanges(cancellationToken);

            return Result.Ok();
        }
    }
}
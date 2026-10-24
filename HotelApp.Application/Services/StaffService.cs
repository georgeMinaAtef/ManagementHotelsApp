using AutoMapper;
using HotelApp.Application.Common;
using HotelApp.Application.Contracts;
using HotelApp.Application.DTOs.Staff;
using HotelApp.Application.Specifications;
using HotelApp.Domain.Contracts;
using HotelApp.Domain.Entities.Hotels;

namespace HotelApp.Application.Services
{
    public class StaffService(IUnitOfWork unitOfWork, IMapper mapper) : IStaffService
    {
        private IGenericRepository<Staff, int> Repo => unitOfWork.GetRepository<Staff, int>();
        private IGenericRepository<Hotel, int> HotelRepo => unitOfWork.GetRepository<Hotel, int>();

        private static Error StaffNotFound(int id)
            => Error.NotFound("Staff.NotFound", $"Staff with id {id} was not found.");

        private static Error HotelNotFound(int id)
            => Error.NotFound("Staff.HotelNotFound", $"Hotel with id {id} was not found.");

        private static Error IsManager(string action)
            => Error.Conflict("Staff.IsManager",
                $"This staff member manages a hotel. Assign another manager before {action}.");

        public async Task<Result<IReadOnlyList<StaffDto>>> GetAllAsync(int? hotelId = null, CancellationToken ct = default)
        {
            var staff = await Repo.GetAllAsync(StaffSpecification.List(hotelId), ct);
            return Result<IReadOnlyList<StaffDto>>.Ok(mapper.Map<IReadOnlyList<StaffDto>>(staff));
        }

        public async Task<Result<StaffDto>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            // قراءة فقط: NoTracking
            var staff = await Repo.GetAsync(StaffSpecification.ByIdReadOnly(id), ct);
            if (staff is null)
                return Result<StaffDto>.Fail(StaffNotFound(id));

            return Result<StaffDto>.Ok(mapper.Map<StaffDto>(staff));
        }

        public async Task<Result<StaffDto>> CreateAsync(CreateStaffDto dto, CancellationToken ct = default)
        {
            if (await HotelRepo.GetByIdAsync(dto.HotelId, ct) is null)
                return Result<StaffDto>.Fail(HotelNotFound(dto.HotelId));

            var staff = mapper.Map<Staff>(dto);

            Repo.Add(staff, ct);
            await unitOfWork.SaveChanges(ct);

            return Result<StaffDto>.Ok(mapper.Map<StaffDto>(staff));
        }

        public async Task<Result<StaffDto>> UpdateAsync(int id, UpdateStaffDto dto, CancellationToken ct = default)
        {
            // متتبَّع (من غير NoTracking) عشان SaveChanges يشوف التعديل
            var staff = await Repo.GetAsync(StaffSpecification.ById(id), ct);
            if (staff is null)
                return Result<StaffDto>.Fail(StaffNotFound(id));

            if (await HotelRepo.GetByIdAsync(dto.HotelId, ct) is null)
                return Result<StaffDto>.Fail(HotelNotFound(dto.HotelId));

            // مدير فندق مينفعش ينتقل لفندق تاني
            if (staff.ManagedHotel is not null && staff.HotelId != dto.HotelId)
                return Result<StaffDto>.Fail(IsManager("moving them"));

            mapper.Map(dto, staff);   // Id متجاهل في StaffProfile

            Repo.Update(staff, ct);
            await unitOfWork.SaveChanges(ct);

            return Result<StaffDto>.Ok(mapper.Map<StaffDto>(staff));
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken ct = default)
        {
            var staff = await Repo.GetAsync(StaffSpecification.ById(id), ct);
            if (staff is null)
                return Result.Fail(StaffNotFound(id));

            if (staff.ManagedHotel is not null)
                return Result.Fail(IsManager("deleting them"));

            Repo.Delete(staff, ct);
            await unitOfWork.SaveChanges(ct);

            return Result.Ok();
        }
    }
}
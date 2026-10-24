using AutoMapper;
using HotelApp.Application.Common;
using HotelApp.Application.Contracts;
using HotelApp.Application.DTOs.Services;
using HotelApp.Application.Specifications;
using HotelApp.Domain.Contracts;
using HotelApp.Domain.Entities.Hotels;

namespace HotelApp.Application.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ServiceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // ---------- الأخطاء الثابتة ----------
        private static readonly Error HotelNotFound =
            new("Service.HotelNotFound", "Hotel was not found.", ErrorType.NotFound);

        private static readonly Error ServiceNotFound =
            new("Service.NotFound", "Service was not found in this hotel.", ErrorType.NotFound);

        private static readonly Error StaffNotFound =
            new("Service.StaffNotFound", "Staff member was not found in this hotel.", ErrorType.NotFound);

        private static readonly Error NameRequired =
            new("Service.NameRequired", "Service name is required.", ErrorType.Validation);

        private static readonly Error InvalidCharge =
            new("Service.InvalidCharge", "Charge cannot be negative.", ErrorType.Validation);

        // جديد
        private static readonly Error InvalidRequestDate =
            new("Service.InvalidRequestDate", "Request date must be later than today's date.", ErrorType.Validation);

        // ---------- GET ALL ----------
        public async Task<Result<IReadOnlyList<ServiceDto>>> GetAllAsync(int hotelId, string? search = null)
        {
            if (!await HotelExistsAsync(hotelId))
                return Result<IReadOnlyList<ServiceDto>>.Fail(HotelNotFound);

            var repo = _unitOfWork.GetRepository<Service, int>();
            var services = await repo.GetAllAsync(new ServicesByHotelSpecification(hotelId, search));

            IReadOnlyList<ServiceDto> dtos = _mapper.Map<List<ServiceDto>>(services);
            return Result<IReadOnlyList<ServiceDto>>.Ok(dtos);
        }

        // ---------- GET BY ID ----------
        public async Task<Result<ServiceDto>> GetByIdAsync(int hotelId, int id)
        {
            var repo = _unitOfWork.GetRepository<Service, int>();
            var service = await repo.GetAsync(new ServiceInHotelSpecification(id, hotelId));

            return service is null
                ? Result<ServiceDto>.Fail(ServiceNotFound)
                : Result<ServiceDto>.Ok(_mapper.Map<ServiceDto>(service));
        }

        // ---------- CREATE ----------
        public async Task<Result<ServiceDto>> CreateAsync(int hotelId, CreateServiceDto dto)
        {
            var validation = Validate(dto.ServiceName, dto.Charge);
            if (validation is not null)
                return Result<ServiceDto>.Fail(validation);

            // جديد: تاريخ الطلب لازم يكون أكبر من الوقت الحالي
            if (!IsInFuture(dto.RequestDate))
                return Result<ServiceDto>.Fail(InvalidRequestDate);

            if (!await HotelExistsAsync(hotelId))
                return Result<ServiceDto>.Fail(HotelNotFound);

            if (!await StaffInHotelAsync(dto.StaffId, hotelId))
                return Result<ServiceDto>.Fail(StaffNotFound);

            var service = _mapper.Map<Service>(dto);

            var repo = _unitOfWork.GetRepository<Service, int>();
            repo.Add(service);
            await _unitOfWork.SaveChanges();

            return Result<ServiceDto>.Ok(_mapper.Map<ServiceDto>(service));
        }

        // ---------- UPDATE ----------
        public async Task<Result> UpdateAsync(int hotelId, int id, UpdateServiceDto dto)
        {
            var repo = _unitOfWork.GetRepository<Service, int>();
            var service = await repo.GetAsync(new ServiceInHotelSpecification(id, hotelId));

            if (service is null)
                return Result.Fail(ServiceNotFound);

            var validation = Validate(dto.ServiceName, dto.Charge);
            if (validation is not null)
                return Result.Fail(validation);

            // جديد: بنفحص التاريخ بس لو اتغير، عشان نقدر نعدل الاسم أو السعر
            // لخدمة تاريخها عدّى من غير ما نتعطل على التاريخ القديم
            if (dto.RequestDate != service.RequestDate && !IsInFuture(dto.RequestDate))
                return Result.Fail(InvalidRequestDate);

            if (!await StaffInHotelAsync(dto.StaffId, hotelId))
                return Result.Fail(StaffNotFound);

            _mapper.Map(dto, service);

            repo.Update(service);
            await _unitOfWork.SaveChanges();

            return Result.Ok();
        }

        // ---------- DELETE ----------
        public async Task<Result> DeleteAsync(int hotelId, int id)
        {
            var repo = _unitOfWork.GetRepository<Service, int>();
            var service = await repo.GetAsync(new ServiceInHotelSpecification(id, hotelId));

            if (service is null)
                return Result.Fail(ServiceNotFound);

            repo.Delete(service);
            await _unitOfWork.SaveChanges();

            return Result.Ok();
        }

        // ---------- Helpers ----------
        private static Error? Validate(string name, decimal charge)
        {
            if (string.IsNullOrWhiteSpace(name)) return NameRequired;
            if (charge < 0) return InvalidCharge;
            return null;
        }

        // المقارنة بالتاريخ بس (من غير الوقت): لازم يكون بعد النهارده
        // لو التاريخ جاي بـ UTC (فيه Z) بنقارنه بـ UtcNow، غير كده بالوقت المحلي للسيرفر
        private static bool IsInFuture(DateTime requestDate)
        {
            var now = requestDate.Kind == DateTimeKind.Utc ? DateTime.UtcNow : DateTime.Now;
            return requestDate.Date >= now.Date;
        }

        private async Task<bool> HotelExistsAsync(int hotelId)
        {
            var repo = _unitOfWork.GetRepository<Hotel, int>();
            return await repo.AnyAsync(new HotelByIdSpecification(hotelId));
        }

        private async Task<bool> StaffInHotelAsync(int staffId, int hotelId)
        {
            var repo = _unitOfWork.GetRepository<Staff, int>();
            return await repo.AnyAsync(new StaffInHotelSpecification(staffId, hotelId));
        }
    }
}
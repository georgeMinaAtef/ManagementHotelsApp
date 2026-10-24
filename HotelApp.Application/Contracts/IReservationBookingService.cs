using HotelApp.Application.Common;
using HotelApp.Application.DTOs.Reservations;

namespace HotelApp.Application.Contracts
{
    public interface IReservationBookingService
    {
        // تأكيد الحجز من Hold موجود + بيانات الأشخاص
        Task<Result<ReservationDto>> CreateAsync(int hotelId, CreateReservationDto dto, CancellationToken c = default);

        Task<Result<ReservationDto>> GetByIdAsync(int hotelId, int id, CancellationToken c = default);

        // تعديل بيانات الأشخاص وعددهم، ولو حبيت تغير الغرفة أو التواريخ
        Task<Result<ReservationDto>> UpdateAsync(int hotelId, int id, UpdateReservationDto dto, CancellationToken c = default);

        // إضافة خدمة للحجز (سعرها بيتضاف على إجمالي الحجز)
        Task<Result<ReservationDto>> AddServiceAsync(int hotelId, int id, int serviceId, CancellationToken c = default);

        // حذف خدمة من الحجز (سعرها بيتخصم من إجمالي الحجز)
        Task<Result<ReservationDto>> RemoveServiceAsync(int hotelId, int id, int serviceId, CancellationToken c = default);
    }
}
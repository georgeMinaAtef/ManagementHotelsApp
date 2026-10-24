using HotelApp.Application.Contracts;
using HotelApp.Application.DTOs.Reservations;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Api.Controllers
{
    // الـ Route الناتج: api/hotels/{hotelId}/reservations
    [Route("api/hotels/{hotelId:int}/reservations")]
    public class ReservationsController : ApiBaseController
    {
        private readonly IReservationBookingService _reservations;

        public ReservationsController(IReservationBookingService reservations)
            => _reservations = reservations;

        // POST api/hotels/1/reservations   (تأكيد الحجز بعد الـ Hold)
        [HttpPost]
        public async Task<ActionResult<ReservationDto>> Create(
            int hotelId, [FromBody] CreateReservationDto dto, CancellationToken c)
            => ToActionResult(await _reservations.CreateAsync(hotelId, dto, c));

        // GET api/hotels/1/reservations/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ReservationDto>> GetById(int hotelId, int id, CancellationToken c)
            => ToActionResult(await _reservations.GetByIdAsync(hotelId, id, c));

        // PUT api/hotels/1/reservations/5   (تعديل الأشخاص / الإقامة)
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ReservationDto>> Update(
            int hotelId, int id, [FromBody] UpdateReservationDto dto, CancellationToken c)
            => ToActionResult(await _reservations.UpdateAsync(hotelId, id, dto, c));

        // POST api/hotels/1/reservations/5/services/3   (إضافة خدمة للحجز)
        [HttpPost("{id:int}/services/{serviceId:int}")]
        public async Task<ActionResult<ReservationDto>> AddService(
            int hotelId, int id, int serviceId, CancellationToken c)
            => ToActionResult(await _reservations.AddServiceAsync(hotelId, id, serviceId, c));

        // DELETE api/hotels/1/reservations/5/services/3   (حذف خدمة من الحجز)
        [HttpDelete("{id:int}/services/{serviceId:int}")]
        public async Task<ActionResult<ReservationDto>> RemoveService(
            int hotelId, int id, int serviceId, CancellationToken c)
            => ToActionResult(await _reservations.RemoveServiceAsync(hotelId, id, serviceId, c));
    }
}
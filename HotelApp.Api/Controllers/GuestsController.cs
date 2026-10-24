using HotelApp.Application.Contracts;
using HotelApp.Application.DTOs.Guest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Api.Controllers
{
    [Route("api/hotels/{hotelId:int}/guests")]   // hotelId always comes from the route
    public class GuestsController(IGuestService service) : ApiBaseController
    {
        // GET api/hotels/1/guests?search=ahmed&nationality=Egyptian&sort=nameAsc
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<GuestDto>>> GetAll(
            int hotelId, [FromQuery] GuestParams p, CancellationToken c)
            => ToActionResult(await service.GetAllAsync(hotelId, p, c));

        // GET api/hotels/1/guests/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<GuestDto>> GetById(int hotelId, int id, CancellationToken c)
            => ToActionResult(await service.GetByIdAsync(hotelId, id, c));

        // POST api/hotels/1/guests
        [HttpPost]
        public async Task<ActionResult<GuestDto>> Create(
            int hotelId, [FromBody] CreateGuestDto dto, CancellationToken c)
        {
            var result = await service.CreateAsync(hotelId, dto, c);

            if (!result.IsSuccess)
                return ToProblem(result.Errors);

            return CreatedAtAction(nameof(GetById),
                new { hotelId, id = result.data!.Id }, result.data);
        }

        // PUT api/hotels/1/guests/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<GuestDto>> Update(
            int hotelId, int id, [FromBody] UpdateGuestDto dto, CancellationToken c)
            => ToActionResult(await service.UpdateAsync(hotelId, id, dto, c));

        // DELETE api/hotels/1/guests/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int hotelId, int id, CancellationToken c)
        {
            var result = await service.DeleteAsync(hotelId, id, c);

            return result.IsSuccess
                ? NoContent()
                : ToProblem(result.Errors);
        }
    }
}

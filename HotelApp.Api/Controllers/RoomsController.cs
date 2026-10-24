using HotelApp.Application.Contracts;
using HotelApp.Application.DTOs.Rooms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Api.Controllers
{
    // [ApiController] and [Route("api/[controller]")] are inherited from ApiBaseController.
    // The "~/" prefix makes each action's route absolute, so the inherited
    // "api/[controller]" route is NOT combined with it (no duplicate routes).
    public class RoomsController(IRoomService roomService) : ApiBaseController
    {
        private const string Base = "~/api/hotels/{hotelId:int}/rooms";

        // GET api/hotels/1/rooms
        [HttpGet(Base)]
        [ProducesResponseType(typeof(IReadOnlyList<RoomDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<RoomDto>>> GetByHotel(int hotelId, CancellationToken ct)
            => ToActionResult(await roomService.GetRoomsByHotelAsync(hotelId, ct));




        // GET api/hotels/1/rooms/5
        [HttpGet(Base + "/{id:int}")]
        [ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoomDto>> GetById(int hotelId, int id, CancellationToken ct)
            => ToActionResult(await roomService.GetRoomByIdAsync(hotelId, id, ct));




        // POST api/hotels/1/rooms
        [HttpPost(Base)]
        [Authorize]
        [ProducesResponseType(typeof(RoomDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<RoomDto>> Create(int hotelId, [FromBody] CreateRoomDto dto, CancellationToken ct)
        {
            var result = await roomService.CreateRoomAsync(hotelId, dto, ct);

            if (!result.IsSuccess)
                return ToProblem(result.Errors);

            return CreatedAtAction(nameof(GetById),
                new { hotelId, id = result.data.RoomId }, result.data);
        }



        // PUT api/hotels/1/rooms/5
        [HttpPut(Base + "/{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<RoomDto>> Update(int hotelId, int id, [FromBody] UpdateRoomDto dto, CancellationToken ct)
            => ToActionResult(await roomService.UpdateRoomAsync(hotelId, id, dto, ct));



        // DELETE api/hotels/1/rooms/5
        [HttpDelete(Base + "/{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Delete(int hotelId, int id, CancellationToken ct)
        {
            var result = await roomService.DeleteRoomAsync(hotelId, id, ct);

            return result.IsSuccess ? NoContent() : ToProblem(result.Errors);
        }
    }
}
using HotelApp.Application.Contracts;
using HotelApp.Application.DTOs.Hotel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Api.Controllers
{
    // [ApiController] and [Route("api/[controller]")] are inherited from ApiBaseController,
    // so they are not repeated here (repeating them would register the route twice).
    public class HotelsController(IHotelService hotelService) : ApiBaseController
    {
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<HotelDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<HotelDto>>> GetAll(CancellationToken cancellationToken)
            => ToActionResult(await hotelService.GetAllHotelsAsync(cancellationToken));

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(HotelDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HotelDetailsDto>> GetById(int id, CancellationToken cancellationToken)
            => ToActionResult(await hotelService.GetHotelByIdAsync(id, cancellationToken));

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(HotelDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<HotelDto>> Create([FromBody] CreateHotelDto dto, CancellationToken cancellationToken)
        {
            var result = await hotelService.CreateHotelAsync(dto, cancellationToken);

            if (!result.IsSuccess)
                return ToProblem(result.Errors);

            return CreatedAtAction(nameof(GetById), new { id = result.data.HotelId }, result.data);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(HotelDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HotelDto>> Update(int id, [FromBody] UpdateHotelDto dto, CancellationToken cancellationToken)
            => ToActionResult(await hotelService.UpdateHotelAsync(id, dto, cancellationToken));

        [HttpDelete("{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var result = await hotelService.DeleteHotelAsync(id, cancellationToken);

            return result.IsSuccess ? NoContent() : ToProblem(result.Errors);
        }

        // ---------------- Manager endpoints ----------------

        [HttpPost("{hotelId:int}/manager/{managerId:int}")]
        [Authorize]
        [ProducesResponseType(typeof(HotelDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<HotelDto>> AddManager(int hotelId, int managerId, CancellationToken cancellationToken)
            => ToActionResult(await hotelService.AddManagerAsync(hotelId, managerId, cancellationToken));

        [HttpPut("{hotelId:int}/manager/{managerId:int}")]
        [Authorize]
        [ProducesResponseType(typeof(HotelDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<HotelDto>> UpdateManager(int hotelId, int managerId, CancellationToken cancellationToken)
            => ToActionResult(await hotelService.UpdateManagerAsync(hotelId, managerId, cancellationToken));

        [HttpDelete("{hotelId:int}/manager")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RemoveManager(int hotelId, CancellationToken cancellationToken)
        {
            var result = await hotelService.RemoveManagerAsync(hotelId, cancellationToken);

            return result.IsSuccess ? NoContent() : ToProblem(result.Errors);
        }
    }
}
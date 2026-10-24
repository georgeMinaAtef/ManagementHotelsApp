using HotelApp.Application.Contracts;
using HotelApp.Application.DTOs.Staff;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Api.Controllers
{
    [Authorize]
    public class StaffController(IStaffService staffService) : ApiBaseController
    {
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<StaffDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<StaffDto>>> GetAll(
            [FromQuery] int? hotelId, CancellationToken ct)
            => ToActionResult(await staffService.GetAllAsync(hotelId, ct));

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(StaffDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StaffDto>> GetById(int id, CancellationToken ct)
            => ToActionResult(await staffService.GetByIdAsync(id, ct));

        [HttpPost]
        [ProducesResponseType(typeof(StaffDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StaffDto>> Create([FromBody] CreateStaffDto dto, CancellationToken ct)
        {
            var result = await staffService.CreateAsync(dto, ct);
            if (!result.IsSuccess)
                return ToProblem(result.Errors);

            return CreatedAtAction(nameof(GetById), new { id = result.data.StaffId }, result.data);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(StaffDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<StaffDto>> Update(int id, [FromBody] UpdateStaffDto dto, CancellationToken ct)
            => ToActionResult(await staffService.UpdateAsync(id, dto, ct));

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await staffService.DeleteAsync(id, ct);
            return result.IsSuccess ? NoContent() : ToProblem(result.Errors);
        }
    }
}

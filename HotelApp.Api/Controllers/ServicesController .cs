using HotelApp.Application.Contracts;
using HotelApp.Application.DTOs.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Api.Controllers
{
    // الـ Route الناتج: api/hotels/{hotelId}/services
    [Route("api/hotels/{hotelId:int}/services")]
    public class ServicesController : ApiBaseController
    {
        private readonly IServiceService _serviceService;

        public ServicesController(IServiceService serviceService)
            => _serviceService = serviceService;

        // GET api/hotels/1/services?search=spa
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ServiceDto>>> GetAll(int hotelId, [FromQuery] string? search)
            => ToActionResult(await _serviceService.GetAllAsync(hotelId, search));

        // GET api/hotels/1/services/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ServiceDto>> GetById(int hotelId, int id)
            => ToActionResult(await _serviceService.GetByIdAsync(hotelId, id));

        // POST api/hotels/1/services
        [HttpPost]
        public async Task<ActionResult<ServiceDto>> Create(int hotelId, [FromBody] CreateServiceDto dto)
            => ToActionResult(await _serviceService.CreateAsync(hotelId, dto));

        // PUT api/hotels/1/services/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int hotelId, int id, [FromBody] UpdateServiceDto dto)
            => ToActionResult(await _serviceService.UpdateAsync(hotelId, id, dto));

        // DELETE api/hotels/1/services/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int hotelId, int id)
            => ToActionResult(await _serviceService.DeleteAsync(hotelId, id));
    }
}
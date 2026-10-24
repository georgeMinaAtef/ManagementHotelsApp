using HotelApp.Application.Contracts;
using HotelApp.Application.DTOs.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelApp.Api.Controllers
{
    public class AuthenticationController : ApiBaseController
    {
        private readonly IAuthenticationsService _authenticationsService;

        public AuthenticationController(IAuthenticationsService authenticationsService)
        {
            this._authenticationsService = authenticationsService;
        }

        [HttpPost("Login")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto, CancellationToken c = default)
        {
            return ToActionResult(await _authenticationsService.LoginAsync(loginDto));
        }

        [HttpPost("Register")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto, CancellationToken c = default)
         => ToActionResult(await _authenticationsService.RegisterAsync(registerDto));



        [HttpGet("emailExists")]
        public async Task<ActionResult<bool>> CheckEmail([FromQuery] string email, CancellationToken c = default)
            => ToActionResult(await _authenticationsService.CheckEmailExistAsync(email, c));



        [Authorize]
        [HttpGet("currentUser")]
        public async Task<ActionResult<UserDto>> GetCurrentUser(CancellationToken c = default)
            => ToActionResult(await _authenticationsService.GetCurrentUserAsync(GetEmailFromToken(), c));



        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDto>> GetCurrentUserAddress(CancellationToken c = default)
            => ToActionResult(await _authenticationsService.GetUserAddressAsync(GetEmailFromToken(), c));

        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto addressDto, CancellationToken c = default)
             => ToActionResult(await _authenticationsService.UpSertUserAddressAsync(GetEmailFromToken(), addressDto, c));

    }
}

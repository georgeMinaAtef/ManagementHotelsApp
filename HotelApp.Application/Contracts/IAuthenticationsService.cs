using HotelApp.Application.Common;
using HotelApp.Application.DTOs.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.Contracts
{
    public interface IAuthenticationsService
    {
        Task<Result<UserDto>> LoginAsync(LoginDto loginDto, CancellationToken c = default);

        Task<Result<UserDto>> RegisterAsync(RegisterDto registerDto, CancellationToken c = default);

        Task<Result<bool>> CheckEmailExistAsync(string email, CancellationToken c = default);

        Task<Result<UserDto>> GetCurrentUserAsync(string email, CancellationToken c = default);

        Task<Result<AddressDto>> GetUserAddressAsync(string email, CancellationToken c = default);

        Task<Result<AddressDto>> UpSertUserAddressAsync(string email, AddressDto addressDto, CancellationToken c = default);
    }
}

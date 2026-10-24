using HotelApp.Application.Common;
using HotelApp.Application.DTOs.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.Contracts
{
    public interface IIdentitytService
    {
        Task<Result<IdentityUserResult>> FindUserByEmailAsync(string email, CancellationToken c = default);

        Task<Result<bool>> CheckPasswordAsync(string email, string password, CancellationToken c = default);

        Task<Result<IdentityUserResult>> CreateUserAsync(RegisterDto registerDto, CancellationToken c = default);

        Task<Result<IReadOnlyList<string>>> GetUserRoles(string email, CancellationToken c = default);

        Task<Result<bool>> EmailExistAsync(string email, CancellationToken c = default);

        Task<Result<AddressDto>> GetUserAddressByEmailAsync(string email, CancellationToken c = default);

        Task<Result<AddressDto>> UpdateOrInsertUserAddressAsync(string email, AddressDto addressDto, CancellationToken c = default);

    }
}

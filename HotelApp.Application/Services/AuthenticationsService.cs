using HotelApp.Application.Common;
using HotelApp.Application.Contracts;
using HotelApp.Application.DTOs.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.Services
{
    internal class AuthenticationsService : IAuthenticationsService
    {
        private readonly IIdentitytService identitytService;
        private readonly ITokenService tokenService;

        public AuthenticationsService(IIdentitytService identitytService, ITokenService tokenService)
        {
            this.identitytService = identitytService;
            this.tokenService = tokenService;
        }

        public async Task<Result<bool>> CheckEmailExistAsync(string email, CancellationToken c = default)
        {
            return await identitytService.EmailExistAsync(email, c);
        }

        public async Task<Result<UserDto>> GetCurrentUserAsync(string email, CancellationToken c = default)
        {
            var userResult = await identitytService.FindUserByEmailAsync(email, c);
            var user = userResult.data;
            var userRole = await identitytService.GetUserRoles(email, c);
            var token = tokenService.CreateToken(user.Id, user.Email, user.UserName, userRole.data);
            return new UserDto()
            {
                Email = email,
                DisplayName = user.DisplayName,
                Token = token,
            };
        }




        public Task<Result<AddressDto>> GetUserAddressAsync(string email, CancellationToken c = default)
        {
            return identitytService.GetUserAddressByEmailAsync(email, c);
        }





        public async Task<Result<UserDto>> LoginAsync(LoginDto loginDto, CancellationToken c = default)
        {
            var userResult = await identitytService.FindUserByEmailAsync(loginDto.Email);
            if (!userResult.IsSuccess)
                return Result<UserDto>.Fail(userResult.Errors);

            var passwordResult = await identitytService.CheckPasswordAsync(loginDto.Email, loginDto.Password, c);
            if (!passwordResult.IsSuccess)
                return Result<UserDto>.Fail(userResult.Errors);

            if (!passwordResult.data)
                return Result<UserDto>.Fail(Error.Unauthorized("Invalid Email Or Password"));


            var user = userResult.data;
            var rolesResult = await identitytService.GetUserRoles(user.Email);
            var roles = rolesResult.data;
            var token = tokenService.CreateToken(user.Id, user.Email, user.UserName, roles);


            return new UserDto()
            {
                Email = loginDto.Email,
                DisplayName = userResult.data.DisplayName,
                Token = token
            };



        }



        public async Task<Result<UserDto>> RegisterAsync(RegisterDto registerDto, CancellationToken c = default)
        {
            var userResult = await identitytService.CreateUserAsync(registerDto);
            if (!userResult.IsSuccess)
                return Result<UserDto>.Fail(userResult.Errors);

            var user = userResult.data;
            var rolesResult = await identitytService.GetUserRoles(user.Email);
            var roles = rolesResult.data;
            var token = tokenService.CreateToken(user.Id, user.Email, user.UserName, roles);


            return Result<UserDto>.Ok(new UserDto()
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = token
            });
        }



        public async Task<Result<AddressDto>> UpSertUserAddressAsync(string email, AddressDto addressDto, CancellationToken c = default)
        {
            return await identitytService.UpdateOrInsertUserAddressAsync(email, addressDto, c);
        }
    }
}

using HotelApp.Application.Common;
using HotelApp.Application.Contracts;
using HotelApp.Application.DTOs.Identity;
using HotelApp.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Infrastructure.Identity.Service
{
    internal class IdentitytService : IIdentitytService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentitytService(UserManager<ApplicationUser> userManager)
        {
            this._userManager = userManager;
        }

        public async Task<Result<bool>> CheckPasswordAsync(string email, string password, CancellationToken c = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result<bool>.Fail(Error.NotFound("User Not Found ", $"User By Email {email} was not found "));
            else
                return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<Result<IdentityUserResult>> CreateUserAsync(RegisterDto registerDto, CancellationToken c = default)
        {
            var user = new ApplicationUser()
            {
                Email = registerDto.Email,
                DisplayName = registerDto.DisplayName,
                UserName = registerDto.UserName,
                PhoneNumber = registerDto.PhoneNumber,

            };
            var result = await _userManager.CreateAsync(user, password: registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => new Error(e.Code, e.Description)).ToList();
                return Result<IdentityUserResult>.Fail(errors);
            }


            return Result<IdentityUserResult>.Ok(new IdentityUserResult(user.Id, user.DisplayName, user.UserName, user.Email));

        }

        public async Task<Result<bool>> EmailExistAsync(string email, CancellationToken c = default)
        {
            return await _userManager.FindByEmailAsync(email) is not null;
        }

        public async Task<Result<IdentityUserResult>> FindUserByEmailAsync(string email, CancellationToken c = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result<IdentityUserResult>.Fail(Error.NotFound("User Not Found ", $"User By Email {email} was not found "));
            else
                return Result<IdentityUserResult>.Ok(new IdentityUserResult(user.Id, user.DisplayName, user.UserName!, user.Email!));
        }

        public async Task<Result<AddressDto>> GetUserAddressByEmailAsync(string email, CancellationToken c = default)
        {
            var user = await _userManager.Users.Include(x => x.Address).FirstOrDefaultAsync(x => x.Email == email, c);
            if (user?.Address == null)
                return Result<AddressDto>.Fail(Error.NotFound("Address Not Found", $"Address With User Email {email} Is Not Exists "));
            var address = user.Address;
            return new AddressDto()
            {
                FirstName = address.FirstName,
                LastName = address.LastName,
                City = address.City,
                Country = address.Country,
                Street = address.Street,
            };
        }

        public async Task<Result<IReadOnlyList<string>>> GetUserRoles(string email, CancellationToken c = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Error.NotFound("User Not Found ", $"User By Email {email} Not Found");
            var result = await _userManager.GetRolesAsync(user);
            return result.ToList();
        }




        public async Task<Result<AddressDto>> UpdateOrInsertUserAddressAsync(string email, AddressDto addressDto, CancellationToken c = default)
        {
            var user = await _userManager.Users.Include(x => x.Address).FirstOrDefaultAsync(x => x.Email == email, c);
            if (user?.Address == null)
            {
                user?.Address = new Address()
                {
                    FirstName = addressDto.FirstName,
                    LastName = addressDto.LastName,
                    City = addressDto.City,
                    Country = addressDto.Country,
                    Street = addressDto.Street,
                };
            }
            else
            {
                user?.Address?.FirstName = addressDto.FirstName;
                user?.Address?.LastName = addressDto.LastName;
                user?.Address?.City = addressDto.City;
                user?.Address?.Country = addressDto.Country;
                user?.Address?.Street = addressDto.Street;
            }

            var result = await _userManager.UpdateAsync(user!);

            if (result.Succeeded)
                return addressDto;
            else
                return Error.Failure("Failure", string.Join(';', result.Errors.Select(e => e.Description)));
        }
    }
}
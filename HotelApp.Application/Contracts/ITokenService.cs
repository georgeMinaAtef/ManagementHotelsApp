using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.Contracts
{
    public interface ITokenService
    {
        string CreateToken(string userId, string email, string userName, IReadOnlyList<string> roles);

    }
}

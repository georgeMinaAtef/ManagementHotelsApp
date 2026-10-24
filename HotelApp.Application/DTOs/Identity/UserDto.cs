using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.DTOs.Identity
{
    public class UserDto
    {
        public string Email { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string Token { get; set; } = default!;
    }
}

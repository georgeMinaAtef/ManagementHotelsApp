using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.Common
{
    public class IdentityUserResult
    {
        public IdentityUserResult(string id, string displayName, string userName, string email)
        {
            Id = id;
            DisplayName = displayName;
            UserName = userName;
            Email = email;
        }

        public string Id { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;

    }
}
    
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Domain.Contracts
{
    public interface IDataSeeder
    {
        Task SeedDataAsync(CancellationToken cancellationToken = default);
    }
}

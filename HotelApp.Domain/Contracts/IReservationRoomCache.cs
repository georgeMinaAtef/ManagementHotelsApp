using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Domain.Contracts
{
    public interface IReservationRoomCache
    {
        Task SetAsync(int reservationId, object roomDetails, TimeSpan? expiry = null, CancellationToken c = default);
    }
}
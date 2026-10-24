using System;
using System.Collections.Generic;
using System.Text;
namespace HotelApp.Infrastructure.Repositories
{
    public static class RedisKeys
    {
        // المفتاح الجديد فيه hotelId
        public static string RoomHold(int hotelId, int roomId, DateOnly date) =>
            $"hotel:{hotelId}:room:{roomId}:date:{date:yyyy-MM-dd}";
    }
}

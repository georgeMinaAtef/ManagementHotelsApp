using HotelApp.Domain.Contracts;
using StackExchange.Redis;
using System.Text.Json;

namespace HotelApp.Infrastructure.Repositories
{
    public class RedisReservationRoomCache(IConnectionMultiplexer redis) : IReservationRoomCache
    {
        private readonly IDatabase _db = redis.GetDatabase();
        public async Task SetAsync(int reservationId, object roomDetails, TimeSpan? expiry = null, CancellationToken c = default)
        {
            var key = $"reservation:{reservationId}:room-details";
            await _db.StringSetAsync(key, JsonSerializer.Serialize(roomDetails), expiry ?? TimeSpan.FromDays(30));
        }
    }
}
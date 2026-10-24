using HotelApp.Domain.Entities.Rooms;

namespace HotelApp.Application.Specifications
{
    public class RoomSpecification : BaseSpecification<Room, int>
    {
        // Private marker used only by the static factory methods at the bottom
        private enum RoomLoad { ReadOnly, ForUpdate, WithReservations }

        // 1) All rooms (with Hotel)
        public RoomSpecification()
        {
            AddInclude(r => r.Hotel);
            ApplyOrderBy(r => r.RoomNumber);
            EnableNoTracking();
        }

        // 2) Room by number inside a hotel (duplicate check, no Include)
        public RoomSpecification(int hotelId, string roomNumber)
            : base(r => r.HotelId == hotelId && r.RoomNumber == roomNumber)
        {
            EnableNoTracking();
        }

        // 3) Rooms of a hotel; onlyAvailable = true returns available rooms only
        public RoomSpecification(int hotelId, bool onlyAvailable)
            : base(r => r.HotelId == hotelId && (!onlyAvailable || r.Availability))
        {
            AddInclude(r => r.Hotel);
            ApplyOrderBy(r => r.RoomNumber);
            EnableNoTracking();
        }

        // 4) Rooms by type (e.g. "Single", "Double", "Suite")
        public RoomSpecification(string roomType)
            : base(r => r.RoomType == roomType)
        {
            AddInclude(r => r.Hotel);
            ApplyOrderBy(r => r.RoomNumber);
            EnableNoTracking();
        }

        // 5) Rooms within a daily-rate range
        public RoomSpecification(decimal minRate, decimal maxRate)
            : base(r => r.DailyRate >= minRate && r.DailyRate <= maxRate)
        {
            AddInclude(r => r.Hotel);
            ApplyOrderBy(r => r.DailyRate);
            EnableNoTracking();
        }

        // 6) Rooms of a hotel with minimum capacity (optionally available only)
        public RoomSpecification(int hotelId, int minCapacity, bool onlyAvailable)
            : base(r => r.HotelId == hotelId
                        && r.Capacity >= minCapacity
                        && (!onlyAvailable || r.Availability))
        {
            AddInclude(r => r.Hotel);
            ApplyOrderBy(r => r.Capacity);
            EnableNoTracking();
        }

        // 7) One room that must belong to the given hotel (hotelId + id)
        private RoomSpecification(int hotelId, int id, RoomLoad load)
            : base(r => r.HotelId == hotelId && r.Id == id)
        {
            switch (load)
            {
                case RoomLoad.ReadOnly:
                    AddInclude(r => r.Hotel);
                    EnableNoTracking();
                    break;

                case RoomLoad.WithReservations:
                    AddInclude(r => r.ReservationRooms);   // tracked
                    break;

                    // RoomLoad.ForUpdate: tracked, no includes
            }
        }

        // Read-only room with its Hotel (for GET)
        public static RoomSpecification ByHotelAndId(int hotelId, int id)
            => new(hotelId, id, RoomLoad.ReadOnly);

        // Tracked room (for UPDATE)
        public static RoomSpecification ForUpdate(int hotelId, int id)
            => new(hotelId, id, RoomLoad.ForUpdate);

        // Tracked room with its reservations (for DELETE)
        public static RoomSpecification WithReservations(int hotelId, int id)
            => new(hotelId, id, RoomLoad.WithReservations);
    }
}
using HotelApp.Domain.Entities.Hotels;

namespace HotelApp.Application.Specifications
{
    public class HotelSpecification : BaseSpecification<Hotel, int>
    {
        // List: GetAllHotelsAsync -> HotelDto (no includes)
        public HotelSpecification()
        {
            ApplyOrderBy(h => h.Id);
            EnableNoTracking();
        }

        // Details: GetHotelByIdAsync -> HotelDetailsDto (needs Manager, Rooms, Staff)
        public HotelSpecification(int id) : base(h => h.Id == id)
        {
            AddInclude(h => h.Manager!);
            AddInclude(h => h.Rooms);
            AddInclude(h => h.Staff);
            EnableSplitQuery();
            EnableNoTracking();
        }
    }
}
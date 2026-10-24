using HotelApp.Domain.Entities.Hotels;

namespace HotelApp.Application.Specifications
{
    // الـ Model ما اتغيرش: الخدمة بتتربط بالفندق عن طريق الموظف (Service.Staff.HotelId)

    // 1) كل خدمات فندق معين + بحث اختياري بالاسم + ترتيب + قراءة فقط
    public class ServicesByHotelSpecification : BaseSpecification<Service, int>
    {
        public ServicesByHotelSpecification(int hotelId, string? search = null)
            : base(s => s.Staff.HotelId == hotelId
                        && (string.IsNullOrEmpty(search) || s.ServiceName.Contains(search)))
        {
            ApplyOrderBy(s => s.ServiceName);
            EnableNoTracking();
        }
    }

    // 2) خدمة واحدة بالـ Id بشرط تكون تابعة للفندق ده
    //    (بدون NoTracking لأننا بنستخدمها في التعديل والحذف)
    public class ServiceInHotelSpecification : BaseSpecification<Service, int>
    {
        public ServiceInHotelSpecification(int id, int hotelId)
            : base(s => s.Id == id && s.Staff.HotelId == hotelId) { }
    }

    // 3) الموظف موجود وتابع للفندق ده (بتتستخدم مع AnyAsync)
    public class StaffInHotelSpecification : BaseSpecification<Staff, int>
    {
        public StaffInHotelSpecification(int staffId, int hotelId)
            : base(s => s.Id == staffId && s.HotelId == hotelId)
        {
            EnableNoTracking();
        }
    }

    // 4) الفندق موجود (بتتستخدم مع AnyAsync)
    public class HotelByIdSpecification : BaseSpecification<Hotel, int>
    {
        public HotelByIdSpecification(int id) : base(h => h.Id == id)
        {
            EnableNoTracking();
        }
    }
}
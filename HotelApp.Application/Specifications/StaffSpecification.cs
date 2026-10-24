using HotelApp.Domain.Entities.Hotels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HotelApp.Application.Specifications
{
    public class StaffSpecification : BaseSpecification<Staff, int>
    {
        private StaffSpecification(Expression<Func<Staff, bool>> criteria) : base(criteria)
        {
            AddInclude(s => s.ManagedHotel!);   // مطلوب لـ IsManager
        }

        /// قايمة الموظفين (قراءة فقط)، مع فلترة اختيارية بالفندق
        public static StaffSpecification List(int? hotelId = null)
        {
            var spec = new StaffSpecification(s => hotelId == null || s.HotelId == hotelId);
            spec.ApplyOrderBy(s => s.FullName);
            spec.EnableNoTracking();
            return spec;
        }

        /// موظف واحد للقراءة فقط (GetById)
        public static StaffSpecification ByIdReadOnly(int id)
        {
            var spec = new StaffSpecification(s => s.Id == id);
            spec.EnableNoTracking();
            return spec;
        }

        /// موظف واحد متتبَّع، للـ Update والـ Delete
        public static StaffSpecification ById(int id)
            => new(s => s.Id == id);
    }
}

using HotelApp.Application.DTOs.Guest;
using HotelApp.Domain.Entities.Guests;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HotelApp.Application.Specifications
{
    // One specification class for every Guest query, created through the static factories below
    public class GuestSpecification : BaseSpecification<Guest, int>
    {
        private GuestSpecification(Expression<Func<Guest, bool>> criteria) : base(criteria) { }

        // GET list (filter + sort), read-only
        public static GuestSpecification ForList(GuestParams p)
        {
            var search = p.Search?.Trim();
            var nationality = p.Nationality?.Trim();

            var spec = new GuestSpecification(g =>
                (string.IsNullOrEmpty(search) ||
                    g.FullName.Contains(search) ||
                    g.PassportNumber.Contains(search)) &&
                (string.IsNullOrEmpty(nationality) || g.Nationality == nationality));

            spec.AddInclude(g => g.ContactDetails);
            spec.EnableNoTracking();
            spec.EnableSplitQuery();

            switch (p.Sort)
            {
                case "nameDesc": spec.ApplyOrderByDescending(g => g.FullName); break;
                case "dobAsc": spec.ApplyOrderBy(g => g.DateOfBirth); break;
                case "dobDesc": spec.ApplyOrderByDescending(g => g.DateOfBirth); break;
                default: spec.ApplyOrderBy(g => g.FullName); break;
            }

            return spec;
        }

        // GET by id (read-only)
        public static GuestSpecification ById(int id)
        {
            var spec = new GuestSpecification(g => g.Id == id);
            spec.AddInclude(g => g.ContactDetails);
            spec.EnableNoTracking();
            return spec;
        }

        // Update: tracked entity (no AsNoTracking) so EF detects the changes
        public static GuestSpecification ForUpdate(int id)
        {
            var spec = new GuestSpecification(g => g.Id == id);
            spec.AddInclude(g => g.ContactDetails);
            return spec;
        }

        // Delete: tracked + reservations, to check the guest isn't used
        public static GuestSpecification ForDelete(int id)
        {
            var spec = new GuestSpecification(g => g.Id == id);
            spec.AddInclude(g => g.ContactDetails);
            spec.AddInclude(g => g.ReservationGuests);
            return spec;
        }

        // Passport is unique (the index in GuestConfiguration); excludeId is used on update
        public static GuestSpecification ByPassport(string passport, int? excludeId = null)
        {
            var spec = new GuestSpecification(g =>
                g.PassportNumber == passport && (excludeId == null || g.Id != excludeId));
            spec.EnableNoTracking();
            return spec;
        }
    }
}

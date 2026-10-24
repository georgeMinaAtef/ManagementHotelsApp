using AutoMapper;
using HotelApp.Application.Common;
using HotelApp.Application.Contracts;
using HotelApp.Application.DTOs.Guest;
using HotelApp.Application.Specifications;
using HotelApp.Domain.Contracts;
using HotelApp.Domain.Entities.Guests;
using HotelApp.Domain.Entities.Hotels;

namespace HotelApp.Application.Services
{
    public class GuestService(IUnitOfWork unitOfWork, IMapper mapper) : IGuestService
    {
        private IGenericRepository<Guest, int> Guests => unitOfWork.GetRepository<Guest, int>();



        public async Task<Result<IReadOnlyList<GuestDto>>> GetAllAsync(int hotelId, GuestParams p, CancellationToken c = default)
        {
            if (await CheckHotelAsync(hotelId, c) is { } hotelError)
                return Fail<IReadOnlyList<GuestDto>>(hotelError);

            var guests = await Guests.GetAllAsync(GuestSpecification.ForList(p), c);
            return Ok(mapper.Map<IReadOnlyList<GuestDto>>(guests));
        }

        public async Task<Result<GuestDto>> GetByIdAsync(int hotelId, int id, CancellationToken c = default)
        {
            if (await CheckHotelAsync(hotelId, c) is { } hotelError)
                return Fail<GuestDto>(hotelError);

            var guest = await Guests.GetAsync(GuestSpecification.ById(id), c);
            if (guest is null)
                return Fail<GuestDto>(GuestNotFound(id));

            return Ok(mapper.Map<GuestDto>(guest));
        }

        public async Task<Result<GuestDto>> CreateAsync(int hotelId, CreateGuestDto dto, CancellationToken c = default)
        {
            if (await CheckHotelAsync(hotelId, c) is { } hotelError)
                return Fail<GuestDto>(hotelError);

            if (await CheckPassportAsync(dto.PassportNumber, null, c) is { } passportError)
                return Fail<GuestDto>(passportError);

            var guest = mapper.Map<Guest>(dto);
            guest.FullName = guest.FullName.Trim();
            guest.Nationality = guest.Nationality.Trim();
            guest.PassportNumber = guest.PassportNumber.Trim();
            guest.DateOfBirth = guest.DateOfBirth.Date;

            foreach (var detail in CleanDetails(dto.ContactDetails))
                guest.ContactDetails.Add(new GuestContactDetail { Detail = detail });

            Guests.Add(guest, c);
            await unitOfWork.SaveChanges(c);

            return Ok(mapper.Map<GuestDto>(guest));
        }

        public async Task<Result<GuestDto>> UpdateAsync(int hotelId, int id, UpdateGuestDto dto, CancellationToken c = default)
        {
            if (await CheckHotelAsync(hotelId, c) is { } hotelError)
                return Fail<GuestDto>(hotelError);

            var guest = await Guests.GetAsync(GuestSpecification.ForUpdate(id), c);
            if (guest is null)
                return Fail<GuestDto>(GuestNotFound(id));

            if (await CheckPassportAsync(dto.PassportNumber, id, c) is { } passportError)
                return Fail<GuestDto>(passportError);

            guest.FullName = dto.FullName.Trim();
            guest.Nationality = dto.Nationality.Trim();
            guest.PassportNumber = dto.PassportNumber.Trim();
            guest.DateOfBirth = guest.DateOfBirth.Date;

            // PK of contact details = (GuestId, Detail): remove the missing ones, add the new ones
            var wanted = CleanDetails(dto.ContactDetails).ToHashSet();

            foreach (var old in guest.ContactDetails.Where(x => !wanted.Contains(x.Detail)).ToList())
                guest.ContactDetails.Remove(old);

            var existing = guest.ContactDetails.Select(x => x.Detail).ToHashSet();
            foreach (var detail in wanted.Where(d => !existing.Contains(d)))
                guest.ContactDetails.Add(new GuestContactDetail { Detail = detail });

            // guest is already tracked (the update spec has no AsNoTracking), so SaveChanges is enough
            await unitOfWork.SaveChanges(c);

            return Ok(mapper.Map<GuestDto>(guest));
        }

        public async Task<Result> DeleteAsync(int hotelId, int id, CancellationToken c = default)
        {
            if (await CheckHotelAsync(hotelId, c) is { } hotelError)
                return Fail(hotelError);

            var guest = await Guests.GetAsync(GuestSpecification.ForDelete(id), c);
            if (guest is null)
                return Fail(GuestNotFound(id));

            if (guest.ReservationGuests.Count > 0)
                return Fail(new Error("Guest.HasReservations",
                    "Cannot delete a guest that is linked to reservations.", ErrorType.Conflict));

            Guests.Delete(guest, c);                       // contact details are removed by cascade
            await unitOfWork.SaveChanges(c);

            return Ok();
        }

        // ---------- checks (return an Error, or null when everything is fine) ----------

        private async Task<Error?> CheckHotelAsync(int hotelId, CancellationToken c)
        {
            var hotel = await unitOfWork.GetRepository<Hotel, int>().GetByIdAsync(hotelId, c);
            return hotel is null
                ? new Error("Hotel.NotFound", $"Hotel {hotelId} not found.", ErrorType.NotFound)
                : null;
        }

        private async Task<Error?> CheckPassportAsync(string passport, int? excludeId, CancellationToken c)
        {
            var exists = await Guests.AnyAsync(GuestSpecification.ByPassport(passport.Trim(), excludeId), c);
            return exists
                ? new Error("Guest.PassportExists",
                    "A guest with this passport number already exists.", ErrorType.Conflict)
                : null;
        }

        private static Error GuestNotFound(int id) =>
            new("Guest.NotFound", $"Guest {id} not found.", ErrorType.NotFound);

        private static IEnumerable<string> CleanDetails(IEnumerable<string>? details) =>
            (details ?? Enumerable.Empty<string>())
                .Select(d => d?.Trim() ?? string.Empty)
                .Where(d => d.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase);

        // ---------- Result helpers: the ONLY place that depends on your Result API ----------
        // If your factory methods have other names, change these four lines only.

        private static Result<T> Ok<T>(T value) => Result<T>.Ok(value);
        private static Result Ok() => Result.Ok();
        private static Result<T> Fail<T>(Error error) => Result<T>.Fail(error);
        private static Result Fail(Error error) => Result.Fail(error);
    }
}
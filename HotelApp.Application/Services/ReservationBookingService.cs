using AutoMapper;
using HotelApp.Application.Common;
using HotelApp.Application.Contracts;
using HotelApp.Application.DTOs.Guest;
using HotelApp.Application.DTOs.Reservations;
using HotelApp.Application.Specifications;
using HotelApp.Domain.Contracts;
using HotelApp.Domain.Entities.Guests;
using HotelApp.Domain.Entities.Hotels;
using HotelApp.Domain.Entities.Reservations;
using HotelApp.Domain.Entities.Rooms;

namespace HotelApp.Application.Services
{
    public class ReservationBookingService(
        IUnitOfWork uow,
        IReservationAvailabilityChecker availability,
        IReservationRoomCache roomCache,
        IMapper mapper) : IReservationBookingService
    {
        private static readonly Error ReservationNotFound = new("Reservation.NotFound", "Reservation was not found in this hotel.", ErrorType.NotFound);
        private static readonly Error RoomBooked = new("Reservation.RoomBooked", "This room is already reserved for some of the selected dates.", ErrorType.Conflict);
        private static readonly Error CapacityExceeded = new("Reservation.CapacityExceeded", "Number of guests exceeds the room capacity.", ErrorType.Validation);

        // ---- أخطاء إضافة/حذف الخدمة ----
        private static readonly Error ReservationCancelled = new("Reservation.Cancelled", "Services cannot be changed on a cancelled reservation.", ErrorType.Validation);
        private static readonly Error ServiceNotFound = new("Service.NotFound", "Service was not found in this hotel.", ErrorType.NotFound);
        private static readonly Error ServiceAlreadyAdded = new("Reservation.ServiceAlreadyAdded", "This service is already added to the reservation.", ErrorType.Conflict);
        private static readonly Error ServiceNotOnReservation = new("Reservation.ServiceNotOnReservation", "This service is not on the reservation.", ErrorType.NotFound);

        // ---- أخطاء التعديل ----
        private static readonly Error AdultsRequired = new("Reservation.AdultsRequired", "At least one adult is required.", ErrorType.Validation);
        private static readonly Error InvalidChildren = new("Reservation.InvalidChildren", "Number of children cannot be negative.", ErrorType.Validation);
        private static readonly Error GuestsRequired = new("Reservation.GuestsRequired", "At least one guest is required.", ErrorType.Validation);
        private static readonly Error TooManyGuests = new("Reservation.TooManyGuests", "Number of guests is greater than adults + children.", ErrorType.Validation);
        private static readonly Error GuestDataRequired = new("Guest.DataRequired", "Full name and passport number are required for every guest.", ErrorType.Validation);
        private static readonly Error DuplicateGuestInRequest = new("Guest.DuplicateInRequest", "The same guest or passport number appears more than once in the request.", ErrorType.Validation);
        private static readonly Error GuestNotOnReservation = new("Guest.NotOnReservation", "One of the guests is not part of this reservation.", ErrorType.NotFound);
        private static readonly Error GuestAlreadyOnReservation = new("Guest.AlreadyOnReservation", "A guest with this passport is already on the reservation. Send its GuestId to edit it.", ErrorType.Conflict);

        public async Task<Result<ReservationDto>> CreateAsync(int hotelId, CreateReservationDto dto, CancellationToken c = default)
        {
            // 1- checkIn >= today (now)
            var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
            if (dto.CheckIn < today)
                return Result<ReservationDto>.Fail(new Error("Reservation.PastDate",
                    $"Check-in {dto.CheckIn} must be today ({today}) or later", ErrorType.Validation));

            // 2- checkOut > checkIn
            if (dto.CheckOut <= dto.CheckIn)
                return Result<ReservationDto>.Fail(new Error("Reservation.InvalidDates",
                    $"Check-out {dto.CheckOut} must be greater than check-in {dto.CheckIn}", ErrorType.Validation));

            var nights = dto.CheckOut.DayNumber - dto.CheckIn.DayNumber;
            if (nights < 1)
                return Result<ReservationDto>.Fail(new Error("Reservation.InvalidNights", "Must be at least 1 night", ErrorType.Validation));

            var room = await uow.GetRepository<Room, int>().GetByIdAsync(dto.RoomId, c);
            if (room == null || room.HotelId != hotelId)
                return Result<ReservationDto>.Fail(new Error("Room.NotInHotel", $"Room {dto.RoomId} not in hotel {hotelId}", ErrorType.NotFound));

            var checkInDt = dto.CheckIn.ToDateTime(TimeOnly.MinValue);
            var checkOutDt = dto.CheckOut.ToDateTime(TimeOnly.MinValue);

            if (await availability.IsRoomBookedAsync(room.Id, checkInDt, checkOutDt, c))
                return Result<ReservationDto>.Fail(RoomBooked);

            var reservation = new Reservation
            {
                BookingDate = DateTime.UtcNow,
                CheckInDate = checkInDt,
                CheckOutDate = checkOutDt,
                ReservationStatus = ReservationStatuses.Confirmed,
                TotalPrice = nights * room.DailyRate,
                NumberOfAdults = dto.NumberOfAdults,
                NumberOfChildren = dto.NumberOfChildren
            };

            reservation.ReservationRooms.Add(new ReservationRoom { RoomId = room.Id });
            foreach (var g in dto.Guests)
            {
                var guest = new Guest
                {
                    FullName = g.FullName.Trim(),
                    PassportNumber = g.PassportNumber.Trim(),
                    Nationality = (g.Nationality ?? "").Trim(),
                    DateOfBirth = g.DateOfBirth
                };
                foreach (var contact in g.ContactDetails.Where(x => !string.IsNullOrWhiteSpace(x)))
                    guest.ContactDetails.Add(new GuestContactDetail { Detail = contact.Trim() });

                reservation.ReservationGuests.Add(new ReservationGuest { Guest = guest });
            }

            uow.GetRepository<Reservation, int>().Add(reservation);
            await uow.SaveChanges(c);

            await roomCache.SetAsync(reservation.Id, new { room.Id, room.RoomNumber, nights }, TimeSpan.FromDays(30), c);

            return Result<ReservationDto>.Ok(await BuildDtoAsync(reservation, c));
        }

        public async Task<Result<ReservationDto>> GetByIdAsync(int hotelId, int id, CancellationToken c = default)
        {
            var reservation = await uow.GetRepository<Reservation, int>().GetAsync(new ReservationInHotelSpecification(id, hotelId), c);
            return reservation is null
                ? Result<ReservationDto>.Fail(ReservationNotFound)
                : Result<ReservationDto>.Ok(await BuildDtoAsync(reservation, c));
        }

        // ===================== UPDATE =====================
        // القائمة اللي جاية هي الحالة النهائية للأشخاص:
        //   GuestId موجود  -> تعديل | GuestId = null -> إضافة | مش موجود في القائمة -> يتشال من الحجز
        // Stay = null -> الغرفة والتواريخ زي ما هم
        public async Task<Result<ReservationDto>> UpdateAsync(int hotelId, int id, UpdateReservationDto dto, CancellationToken c = default)
        {
            // 1- فحص العدد وبيانات الأشخاص (قبل أي استعلام)
            if (dto.NumberOfAdults < 1)
                return Result<ReservationDto>.Fail(AdultsRequired);
            if (dto.NumberOfChildren < 0)
                return Result<ReservationDto>.Fail(InvalidChildren);
            if (dto.Guests.Count == 0)
                return Result<ReservationDto>.Fail(GuestsRequired);
            if (dto.Guests.Count > dto.NumberOfAdults + dto.NumberOfChildren)
                return Result<ReservationDto>.Fail(TooManyGuests);

            var guestsError = ValidateGuests(dto.Guests);
            if (guestsError is not null)
                return Result<ReservationDto>.Fail(guestsError);

            // 2- الحجز (تابع للفندق + Tracking)
            var reservation = await uow.GetRepository<Reservation, int>()
                .GetAsync(new ReservationInHotelSpecification(id, hotelId), c);
            if (reservation is null)
                return Result<ReservationDto>.Fail(ReservationNotFound);

            if (reservation.ReservationStatus == ReservationStatuses.Cancelled)
                return Result<ReservationDto>.Fail(ReservationCancelled);

            // 3- تغيير الإقامة (الغرفة / التواريخ) لو مطلوب
            Room? changedRoom = null;
            var changedNights = 0;

            if (dto.Stay is not null)
            {
                var currentRoomId = reservation.ReservationRooms.FirstOrDefault()?.RoomId ?? 0;
                var currentIn = DateOnly.FromDateTime(reservation.CheckInDate);
                var currentOut = DateOnly.FromDateTime(reservation.CheckOutDate);

                var roomId = dto.Stay.RoomId ?? currentRoomId;
                var checkIn = dto.Stay.CheckIn ?? currentIn;
                var checkOut = dto.Stay.CheckOut ?? currentOut;

                var roomChanged = roomId != currentRoomId;
                var datesChanged = checkIn != currentIn || checkOut != currentOut;

                if (roomChanged || datesChanged)
                {
                    // نمنع تاريخ الدخول في الماضي بس لو هو اللي اتغير (عشان نقدر نعدل حجز بدأ فعلًا)
                    var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
                    if (checkIn != currentIn && checkIn < today)
                        return Result<ReservationDto>.Fail(new Error("Reservation.PastDate",
                            $"Check-in {checkIn} must be today ({today}) or later", ErrorType.Validation));

                    if (checkOut <= checkIn)
                        return Result<ReservationDto>.Fail(new Error("Reservation.InvalidDates",
                            $"Check-out {checkOut} must be greater than check-in {checkIn}", ErrorType.Validation));

                    var room = await uow.GetRepository<Room, int>().GetByIdAsync(roomId, c);
                    if (room is null || room.HotelId != hotelId)
                        return Result<ReservationDto>.Fail(new Error("Room.NotInHotel",
                            $"Room {roomId} not in hotel {hotelId}", ErrorType.NotFound));

                    var checkInDt = checkIn.ToDateTime(TimeOnly.MinValue);
                    var checkOutDt = checkOut.ToDateTime(TimeOnly.MinValue);

                    // بنستثني الحجز نفسه عشان ما يتعارضش مع نفسه
                    var overlaps = await uow.GetRepository<Reservation, int>()
                        .AnyAsync(new OverlappingReservationSpecification(room.Id, checkInDt, checkOutDt, reservation.Id), c);
                    if (overlaps)
                        return Result<ReservationDto>.Fail(RoomBooked);

                    reservation.CheckInDate = checkInDt;
                    reservation.CheckOutDate = checkOutDt;

                    if (roomChanged)
                    {
                        foreach (var old in reservation.ReservationRooms.ToList())
                            reservation.ReservationRooms.Remove(old);
                        reservation.ReservationRooms.Add(new ReservationRoom { RoomId = room.Id });
                    }

                    changedRoom = room;
                    changedNights = checkOut.DayNumber - checkIn.DayNumber;
                }
            }

            // 4- الأشخاص: تعديل / إضافة / حذف
            var syncError = await SyncGuestsAsync(reservation, dto.Guests, c);
            if (syncError is not null)
                return Result<ReservationDto>.Fail(syncError);

            // 5- العدد + إعادة حساب الإجمالي (الغرفة + الخدمات)
            reservation.NumberOfAdults = dto.NumberOfAdults;
            reservation.NumberOfChildren = dto.NumberOfChildren;
            reservation.TotalPrice = await CalculateTotalAsync(reservation, c);

            uow.GetRepository<Reservation, int>().Update(reservation);
            await uow.SaveChanges(c);

            // 6- تحديث الكاش لو الغرفة أو التواريخ اتغيروا
            if (changedRoom is not null)
                await roomCache.SetAsync(reservation.Id,
                    new { changedRoom.Id, changedRoom.RoomNumber, nights = changedNights },
                    TimeSpan.FromDays(30), c);

            return Result<ReservationDto>.Ok(await BuildDtoAsync(reservation, c));
        }

        private static Error? ValidateGuests(List<UpdateGuestDto> guests)
        {
            if (guests.Any(g => string.IsNullOrWhiteSpace(g.FullName) || string.IsNullOrWhiteSpace(g.PassportNumber)))
                return GuestDataRequired;

            var passports = guests.Select(g => g.PassportNumber.Trim()).ToList();
            if (passports.Distinct(StringComparer.OrdinalIgnoreCase).Count() != passports.Count)
                return DuplicateGuestInRequest;

            var ids = guests.Where(g => g.GuestId.HasValue).Select(g => g.GuestId!.Value).ToList();
            if (ids.Distinct().Count() != ids.Count)
                return DuplicateGuestInRequest;

            return null;
        }

        private async Task<Error?> SyncGuestsAsync(Reservation reservation, List<UpdateGuestDto> incoming, CancellationToken c)
        {
            // الأشخاص الحاليين على الحجز (Tracking + بيانات التواصل)
            var linkedIds = reservation.ReservationGuests.Select(x => x.GuestId).ToArray();
            var linked = linkedIds.Length == 0
                ? new List<Guest>()
                : (await uow.GetRepository<Guest, int>().GetAllAsync(new GuestsByIdsSpecification(linkedIds), c)).ToList();
            var linkedById = linked.ToDictionary(g => g.Id);

            var keepIds = incoming.Where(g => g.GuestId.HasValue).Select(g => g.GuestId!.Value).ToHashSet();
            if (keepIds.Any(x => !linkedById.ContainsKey(x)))
                return GuestNotOnReservation;

            // حذف: اللي مش في القائمة بيتشال من الحجز (الشخص نفسه بيفضل في الجدول)
            foreach (var link in reservation.ReservationGuests.Where(x => !keepIds.Contains(x.GuestId)).ToList())
                reservation.ReservationGuests.Remove(link);

            // تعديل
            foreach (var g in incoming.Where(x => x.GuestId.HasValue))
            {
                var guest = linkedById[g.GuestId!.Value];
                mapper.Map(g, guest);
                SyncContacts(guest, g.ContactDetails);
            }

            // إضافة
            var added = incoming.Where(x => !x.GuestId.HasValue).ToList();
            if (added.Count > 0)
            {
                var passports = added.Select(x => x.PassportNumber.Trim()).Distinct().ToArray();
                var known = (await uow.GetRepository<Guest, int>()
                    .GetAllAsync(new GuestsByPassportsSpecification(passports), c)).ToList();

                foreach (var g in added)
                {
                    var passport = g.PassportNumber.Trim();

                    if (linked.Any(x => x.PassportNumber == passport))
                        return GuestAlreadyOnReservation;

                    // لو الشخص موجود قبل كده بنفس الباسبور بنربطه بدل ما نكرره
                    var guest = known.FirstOrDefault(x => x.PassportNumber == passport);
                    if (guest is null)
                    {
                        guest = mapper.Map<Guest>(g);
                        SyncContacts(guest, g.ContactDetails);
                    }

                    reservation.ReservationGuests.Add(new ReservationGuest { Guest = guest });
                }
            }

            return null;
        }

        // مقارنة بيانات التواصل: نشيل اللي اتشال ونضيف الجديد بس
        private static void SyncContacts(Guest guest, IEnumerable<string> details)
        {
            var wanted = details
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct()
                .ToList();

            foreach (var old in guest.ContactDetails.Where(x => !wanted.Contains(x.Detail)).ToList())
                guest.ContactDetails.Remove(old);

            foreach (var d in wanted.Where(d => guest.ContactDetails.All(x => x.Detail != d)))
                guest.ContactDetails.Add(new GuestContactDetail { Detail = d });
        }

        // ===================== ADD SERVICE =====================
        public async Task<Result<ReservationDto>> AddServiceAsync(int hotelId, int id, int serviceId, CancellationToken c = default)
        {
            // الحجز تابع للفندق + محمّل معاه ReservationServices (Tracking شغال)
            var reservation = await uow.GetRepository<Reservation, int>()
                .GetAsync(new ReservationInHotelSpecification(id, hotelId), c);
            if (reservation is null)
                return Result<ReservationDto>.Fail(ReservationNotFound);

            if (reservation.ReservationStatus == ReservationStatuses.Cancelled)
                return Result<ReservationDto>.Fail(ReservationCancelled);

            // الخدمة لازم تكون تابعة لنفس الفندق (نفس الـ spec اللي في ServiceService)
            var service = await uow.GetRepository<Service, int>()
                .GetAsync(new ServiceInHotelSpecification(serviceId, hotelId), c);
            if (service is null)
                return Result<ReservationDto>.Fail(ServiceNotFound);

            if (reservation.ReservationServices.Any(x => x.ServiceId == serviceId))
                return Result<ReservationDto>.Fail(ServiceAlreadyAdded);

            reservation.ReservationServices.Add(new ReservationService { ServiceId = serviceId });

            // الإجمالي = سعر الغرفة + أسعار كل الخدمات
            reservation.TotalPrice = await CalculateTotalAsync(reservation, c);

            uow.GetRepository<Reservation, int>().Update(reservation);
            await uow.SaveChanges(c);

            return Result<ReservationDto>.Ok(await BuildDtoAsync(reservation, c));
        }

        // ===================== REMOVE SERVICE =====================
        public async Task<Result<ReservationDto>> RemoveServiceAsync(int hotelId, int id, int serviceId, CancellationToken c = default)
        {
            var reservation = await uow.GetRepository<Reservation, int>()
                .GetAsync(new ReservationInHotelSpecification(id, hotelId), c);
            if (reservation is null)
                return Result<ReservationDto>.Fail(ReservationNotFound);

            if (reservation.ReservationStatus == ReservationStatuses.Cancelled)
                return Result<ReservationDto>.Fail(ReservationCancelled);

            var link = reservation.ReservationServices.FirstOrDefault(x => x.ServiceId == serviceId);
            if (link is null)
                return Result<ReservationDto>.Fail(ServiceNotOnReservation);

            reservation.ReservationServices.Remove(link);

            reservation.TotalPrice = await CalculateTotalAsync(reservation, c);

            uow.GetRepository<Reservation, int>().Update(reservation);
            await uow.SaveChanges(c);

            return Result<ReservationDto>.Ok(await BuildDtoAsync(reservation, c));
        }

        // الإجمالي = (عدد الليالي × سعر الغرفة) + مجموع أسعار الخدمات الحالية.
        // بنحسبه من الصفر كل مرة عشان لو سعر خدمة اتغير ما يحصلش فرق في الحساب.
        private async Task<decimal> CalculateTotalAsync(Reservation r, CancellationToken c)
        {
            var nights = DateOnly.FromDateTime(r.CheckOutDate).DayNumber - DateOnly.FromDateTime(r.CheckInDate).DayNumber;

            var roomId = r.ReservationRooms.FirstOrDefault()?.RoomId ?? 0;
            var room = roomId == 0 ? null : await uow.GetRepository<Room, int>().GetByIdAsync(roomId, c);
            var roomTotal = room is null ? 0m : nights * room.DailyRate;

            var serviceIds = r.ReservationServices.Select(x => x.ServiceId).ToArray();
            var servicesTotal = 0m;
            if (serviceIds.Length > 0)
            {
                var services = await uow.GetRepository<Service, int>().GetAllAsync(new ServicesByIdsSpecification(serviceIds), c);
                servicesTotal = services.Sum(s => s.Charge);
            }

            return roomTotal + servicesTotal;
        }

        // ===================== BuildDto =====================
        private async Task<ReservationDto> BuildDtoAsync(Reservation r, CancellationToken c)
        {
            var roomId = r.ReservationRooms.FirstOrDefault()?.RoomId ?? 0;
            var room = roomId == 0 ? null : await uow.GetRepository<Room, int>().GetByIdAsync(roomId, c);

            var guestIds = r.ReservationGuests.Select(x => x.GuestId).ToArray();
            var guests = guestIds.Length == 0
                ? new List<Guest>()
                : await uow.GetRepository<Guest, int>().GetAllAsync(new GuestsByIdsSpecification(guestIds), c);

            var serviceIds = r.ReservationServices.Select(x => x.ServiceId).ToArray();
            var services = serviceIds.Length == 0
                ? new List<Service>()
                : await uow.GetRepository<Service, int>().GetAllAsync(new ServicesByIdsSpecification(serviceIds), c);

            var checkIn = DateOnly.FromDateTime(r.CheckInDate);
            var checkOut = DateOnly.FromDateTime(r.CheckOutDate);
            var nightsCalc = checkOut.DayNumber - checkIn.DayNumber;

            return new ReservationDto
            {
                Id = r.Id,
                Status = r.ReservationStatus,
                BookingDate = r.BookingDate,
                RoomId = roomId,
                RoomNumber = room?.RoomNumber ?? "",
                CheckIn = checkIn,
                CheckOut = checkOut,
                Nights = nightsCalc,
                NumberOfAdults = r.NumberOfAdults,
                NumberOfChildren = r.NumberOfChildren,
                TotalPrice = r.TotalPrice,
                Guests = guests.Select(g => new GuestDto
                {
                    Id = g.Id,
                    FullName = g.FullName,
                    Nationality = g.Nationality,
                    PassportNumber = g.PassportNumber,
                    DateOfBirth = g.DateOfBirth,
                    ContactDetails = g.ContactDetails.Select(x => x.Detail).ToList()
                }).ToList(),
                Services = services.Select(s => new ReservedServiceDto
                {
                    ServiceId = s.Id,
                    ServiceName = s.ServiceName,
                    Charge = s.Charge
                }).ToList()
            };
        }
    }
}
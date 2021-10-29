using System;
using Booking.API.Data;
using Booking.Core.Domain.Entities;

namespace Booking.API.Tests.Utils
{
    public static class Utilities
    {
        public static void InitializeDbForTests(this BookingContext db)
        {
            db.InsertReservations(GetReservationSeeding());
        }

        public static void InsertReservations(this BookingContext db, Reservation[] data)
        {
            db.Reservations.AddRange(data);
            db.SaveChanges();
        }

        public static Reservation InsertReservation(this BookingContext db, Reservation entity)
        {
            var entry = db.Add(entity);
            db.SaveChanges();
            return entry.Entity;
        }

        public static void ReInitializeDbForTests(this BookingContext db)
        {
            db.RemoveRange(db.Reservations);
            db.InitializeDbForTests();
        }

        public static Reservation[] GetReservationSeeding() => new Reservation[]
        {
            new Reservation { Start = GetDate(-2), Days = 3, Finish = GetDate(0) },
            new Reservation { Start = GetDate(2), Days = 3, Finish = GetDate(4) },
            new Reservation { Start = GetDate(7), Days = 3, Finish = GetDate(9) },
            new Reservation { Start = GetDate(12), Days = 2, Finish = GetDate(13) },
            new Reservation { Start = GetDate(17), Days = 1, Finish = GetDate(17) }
        };

        public static DateTime GetDate(int days) => DateTime.UtcNow.Date.AddDays(days);
        public static string GetStringDate(int days) => GetDate(days).ToString(D_FORMAT);

        public const string D_FORMAT = "yyyy-MM-dd";
    }
}
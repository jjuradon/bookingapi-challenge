using System;
using Booking.Core.Domain.Entities;
using Booking.Core.Models;

namespace Booking.Core.Extensions
{
    public static class ReservationExtensions
    {
        public static Reservation CreateEntity(this ReservationModel model)
        {
            var sDate = DateTime.Parse(model.Date);
            return new Reservation
            {
                Start = sDate.Date,
                Days = model.Days,
                Finish = sDate.AddDays(model.Days-1).Date
            };
        }

        public static ReservationResponseModel ToResponse(this Reservation entity) => new ReservationResponseModel
        {
            Id = entity.Id,
            EndDate = entity.Finish.ToString("yyyy-MM-dd"),
            Days = entity.Days,
            Date = entity.Start.ToString("yyyy-MM-dd")
        };

        public static void UpdateEntity(this Reservation entity, ReservationModel model)
        {
            var sDate = DateTime.Parse(model.Date);
            entity.Start = sDate.Date;
            entity.Days = model.Days;
            entity.Finish = sDate.AddDays(model.Days - 1).Date;
        }
    }
}
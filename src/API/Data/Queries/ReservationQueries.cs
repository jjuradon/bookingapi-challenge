using System;
using System.Linq;
using System.Threading.Tasks;
using Booking.Core.Domain.Entities;
using Booking.Core.Exceptions;
using Booking.Core.Extensions;
using Booking.Core.Interfaces;
using Booking.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking.API.Data
{
    public class ReservationQueries : Repository<Reservation>, IReservationRepository
    {
        public ReservationQueries(BookingContext context) : base(context)
        {
        }

        /// <summary>
        /// Get a Reservation using the id.
        /// </summary>
        /// <param name="id">Reservation id.</param>
        /// <returns>Returns a Reservation object.</returns>
        public async Task<Reservation> GetById(int id)
        {
            var entity = await _query.SingleOrDefaultAsync(x => x.Id == id);
            if (entity is null)
                throw new NotFoundServiceException("Reservation not found.");
            if (entity.Start <= DateTime.UtcNow.Date)
                throw new ConflictServiceException($"The reservation cannot be deleted or modified. Started {entity.Start.ToString("yyyy-MM-dd")}");
            return entity;
        }

        /// <summary>
        /// Compares the model's dates if one of them cross with current reservations. Self doesn't count.
        /// </summary>
        /// <param name="entity">Reservation to update.</param>
        /// <returns>Returns True if there isn't a reservation with the given information.</returns>
        public async Task<bool> IsAvailableForUpdate(Reservation entity)
        {
            return !await _query.Where(x =>
                (entity.Start >= x.Start && entity.Start <= x.Finish) ||
                (entity.Finish >= x.Start && entity.Finish <= x.Finish) ||
                (x.Start >= entity.Start && x.Start <= entity.Finish && x.Days == 1))
                .AnyAsync(c => c.Id != entity.Id);
        }

        /// <summary>
        /// Compares the model's dates if one of them cross with current reservations.
        /// </summary>
        /// <param name="model">Object with reservation information.</param>
        /// <returns>Returns True if there isn't a reservation with the given information.</returns>
        public async Task<bool> IsAvailableOn(ReservationModel model)
        {
            var entity = model.CreateEntity();
            return !await _query.AnyAsync(x => 
                (entity.Start >= x.Start && entity.Start <= x.Finish) ||
                (entity.Finish >= x.Start && entity.Finish <= x.Finish) ||
                (x.Start >= entity.Start && x.Start <= entity.Finish && x.Days == 1));
        }
    }
}
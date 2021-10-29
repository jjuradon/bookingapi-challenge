using System;
using System.Threading.Tasks;
using Booking.Core.Models;

namespace Booking.Core.Interfaces
{
    /// <summary>
    /// Booking service interface.
    /// </summary>
    public interface IBookingService
    {
        /// <summary>
        /// Check the room availability in a specific date.
        /// </summary>
        /// <param name="model">Object with reservation information.</param>
        Task CheckAvailability(ReservationModel model);
        /// <summary>
        /// Place a reservation using ReservationModel object.
        /// </summary>
        /// <param name="model">Object with reservation information.</param>
        /// <returns>A object with the reservation created information.</returns>
        Task<ReservationResponseModel> PlaceReservation(ReservationModel model);
        /// <summary>
        /// Cancels a Reservation with the given id.
        /// </summary>
        /// <param name="id">Reservation Id.</param>
        Task CancelReservation(int id);
        /// <summary>
        /// Updates a Reservation with the given id.
        /// </summary>
        /// <param name="id">Reservation id.</param>
        /// <param name="model">Updated information.</param>
        /// <returns>Object with the updated reservation information.</returns>
        Task<ReservationResponseModel> UpdateReservation(int id, ReservationModel model);
    }
}
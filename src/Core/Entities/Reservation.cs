using System;

namespace Booking.Core.Domain.Entities
{
    /// <summary>
    /// Class representation for an user placed reservation
    /// </summary>
    public class Reservation
    {
        /// <summary>
        /// Identifier for a reservation
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Staying days
        /// </summary>
        public int Days { get; set; }
        /// <summary>
        /// Reservation staring date
        /// </summary>
        public DateTime Start { get; set; }
        /// <summary>
        /// Reservation ending date (Calculated)
        /// </summary>
        public DateTime Finish { get; set; }
    }
}
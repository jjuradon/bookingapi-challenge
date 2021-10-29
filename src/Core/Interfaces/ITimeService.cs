using System;

namespace Booking.Core.Interfaces
{
    /// <summary>
    /// Support service to handle date operations.
    /// </summary>
    public interface ITimeService
    {
        /// <summary>
        /// Compare the given date is in the next given days.
        /// </summary>
        /// <param name="days">Next days.</param>
        /// <param name="date">Date to be compared.</param>
        /// <returns>Return True if given date is next given days. Otherwise False</returns>
        bool IsIn(int days, string date);

        /// <summary>
        /// Give the date after given days as string.
        /// </summary>
        /// <param name="days">Days to be added.</param>
        /// <returns>A string with the date after given days in format yyyy-MM-dd.</returns>
        string GetDateAfter(int days);
    }
}
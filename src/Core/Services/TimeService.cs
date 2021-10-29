using System;
using Booking.Core.Interfaces;

namespace Booking.Core.Services
{
    public class TimeService : ITimeService
    {
        private readonly DateTime _systemDate;
        public TimeService()
        {
            _systemDate = DateTime.UtcNow.Date;
        }

        public TimeService(string date)
        {
            _systemDate = DateTime.Parse(date);
        }

        public string GetDateAfter(int days)
        {
            var eDate = _systemDate.AddDays(days);
            return eDate.ToString("yyyy-MM-dd");
        }

        public bool IsIn(int days, string date)
        {
            var sDate = DateTime.Parse(date);

            if (sDate < _systemDate)
                throw new InvalidOperationException("The date cannot be before the system date.");

            var eDate = _systemDate.AddDays(30);
            return sDate < eDate;
        }
    }
}
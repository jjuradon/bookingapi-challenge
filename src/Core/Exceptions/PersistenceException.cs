using System;

namespace Booking.Core.Exceptions
{
    public class PersistenceException : Exception
    {
        public PersistenceException() : base() { }
        public PersistenceException(string message) : base(message) { }
        public PersistenceException(string message, Exception innerException) : base(message, innerException) { }
    }
}
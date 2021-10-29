using System;
using System.Runtime.Serialization;

namespace Booking.Core.Exceptions
{
    [Serializable]
    public class ConflictServiceException : Exception
    {
        public ConflictServiceException()
        {
        }

        public ConflictServiceException(string message) : base(message)
        {
        }

        public ConflictServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ConflictServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
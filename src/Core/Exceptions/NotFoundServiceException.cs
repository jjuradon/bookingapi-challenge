using System;
using System.Runtime.Serialization;

namespace Booking.Core.Exceptions
{
    [Serializable]
    public class NotFoundServiceException : Exception
    {
        public NotFoundServiceException()
        {
        }

        public NotFoundServiceException(string message) : base(message)
        {
        }

        public NotFoundServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotFoundServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

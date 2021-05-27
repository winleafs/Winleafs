using System;
using System.Runtime.Serialization;

namespace Winleafs.Api.Exceptions
{
    [Serializable]
    public class InvalidDeviceException : Exception
    {
        public InvalidDeviceException()
        {
        }

        public InvalidDeviceException(string message) : base(message)
        {
        }

        public InvalidDeviceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidDeviceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Winleafs.Models.Exceptions
{
    [Serializable]
    public class InvalidTriggerTimeException : Exception
    {
        public InvalidTriggerTimeException()
        {
        }

        public InvalidTriggerTimeException(string message) : base(message)
        {
        }

        public InvalidTriggerTimeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidTriggerTimeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

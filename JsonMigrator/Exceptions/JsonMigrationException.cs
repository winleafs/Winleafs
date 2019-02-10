using System;

namespace JsonMigrator.Exceptions
{
    public class JsonMigrationException : Exception
    {
        public JsonMigrationException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}

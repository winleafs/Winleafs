using System;

namespace JsonMigrator.Exceptions
{
    public class JsonMigrationVersionException : Exception
    {
        public JsonMigrationVersionException(string message) : base(message)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace JsonMigrator
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class MigrationAttribute : Attribute
    {
        public string FromVersion { get; set; }
        public string ToVersion { get; set; }

        public MigrationAttribute(string fromVersion, string toVersion)
        {
            FromVersion = fromVersion;
            ToVersion = toVersion;
        }
    }
}

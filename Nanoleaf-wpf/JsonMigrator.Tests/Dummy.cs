using Newtonsoft.Json.Linq;

namespace JsonMigrator.Tests
{
    public class Dummy
    {
        public string JsonVersion { get; set; }

        public string StringValue { get; set; }

        public int IntegerValue { get; set; }

        [Migration("1", "2")]
        private void Migration_NonStatic(JToken jToken)
        {

        }
    }
}

using JsonMigrator.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace JsonMigrator.Tests
{
    [TestClass]
    public class ExceptionTests
    {
        [TestMethod]
        [ExpectedException(typeof(JsonMigrationVersionException))]
        public void MissingVersionTest()
        {
            var jtoken = JToken.Parse("{\"StringValue\":\"String1\",\"IntegerValue\":1}");
            JsonMigrator.Migrate<Dummy>(jtoken);
        }
    }
}

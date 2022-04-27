
using Newtonsoft.Json;

namespace Winleafs.Models.State
{
    public class StateModel
    {
        [JsonProperty("value")]
        public int Value { get; set; }

        [JsonProperty("max")]
        public int Max { get; set; }

        [JsonProperty("min")]
        public int Min { get; set; }
    }
}

using Newtonsoft.Json;

namespace Nanoleaf_Models.Models.State
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
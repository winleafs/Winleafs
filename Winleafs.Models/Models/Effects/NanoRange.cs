using Newtonsoft.Json;

namespace Winleafs.Models.Models.Effects
{
    public class NanoRange
    {
        [JsonProperty("maxValue")]
        public int MaxValue { get; set; }

        [JsonProperty("minValue")]
        public int MinValue { get; set; }
    }
}

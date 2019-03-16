using Newtonsoft.Json;

namespace Winleafs.Models.Models.Layouts
{
    public class GlobalOrientation
    {
        [JsonProperty("value")]
        public int Value { get; set; }

        [JsonProperty("min")]
        public int Min { get; set; }

        [JsonProperty("max")]
        public int Max { get; set; }
    }
}

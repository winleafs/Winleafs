using Newtonsoft.Json;

namespace Winleafs.Models.Models.Effects
{
    public class Palette
    {
        [JsonProperty("hue")]
        public int Hue { get; set; }

        [JsonProperty("saturation")]
        public int Saturation { get; set; }

        [JsonProperty("brightness")]
        public int Brightness { get; set; }
    }
}

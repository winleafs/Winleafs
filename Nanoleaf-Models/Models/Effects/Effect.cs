using System.Collections.Generic;

using Newtonsoft.Json;

namespace Nanoleaf_Models.Models.Effects
{
    public class Effect
    {
        [JsonProperty("animName")]
        public string Name { get; set; }

        [JsonProperty("loop")]
        public bool IsOnLoop { get; set; }

        [JsonProperty("pallete")]
        public IEnumerable<Pallete> Pallete { get; set; }

        [JsonProperty("colorType")]
        public string ColorType { get; set; }

        [JsonProperty("animType")]
        public string AnimationType { get; set; }

        [JsonProperty("transTime")]
        public NanoRange TransitionTime { get; set; }

        [JsonProperty("delayTime")]
        public NanoRange DelayTime { get; set; }

        [JsonProperty("explodeFactor")]
        public int ExplodeFactor { get; set; }

        [JsonProperty("brightnessRange")]
        public NanoRange Brightness { get; set; }

        [JsonProperty("direction")]
        public string Direction { get; set; }
    }
}

using System.Collections.Generic;

using Newtonsoft.Json;

namespace Winleafs.Models.Models.Effects
{
    public class Effect
    {
        [JsonProperty("animName")]
        public string Name { get; set; }

        [JsonProperty("loop")]
        public bool IsOnLoop { get; set; }

        [JsonProperty("palette")]
        public IEnumerable<Palette> Palette { get; set; }

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

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            var effect = obj as Effect;

            if (effect == null)
            {
                return false;
            }

            return ((Effect)obj).Name == Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}

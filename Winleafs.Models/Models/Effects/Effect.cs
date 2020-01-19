using System.Collections.Generic;

using Newtonsoft.Json;
using Winleafs.Models.Enums;

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

        [JsonProperty("pluginType")]
        public string PluginType { get; set; }

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
            if (obj is Effect effect)
            {
                return effect.Name == Name;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        /// <summary>
        /// Translates the <see cref="PluginType"/> given by the Nanoleaf API to
        /// an <see cref="EffectType"/>.
        /// </summary>
        [JsonIgnore]
        public EffectType EffectType
        {
            get
            {
                switch (PluginType?.ToLower())
                {
                    case "color":
                        return EffectType.Color;
                    case "rhythm":
                        return EffectType.Rhythm;
                    default:
                        return EffectType.Unknown;
                }
            }
        }
    }
}

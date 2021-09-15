using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winleafs.Models.Models.Effects
{
    public class CustomEffectCommand
    {
        [JsonProperty("command")]
        public string Command { get; set; } // add, update, display

        [JsonProperty("version")]
        public string Version { get; } = "1.0";

        [JsonProperty("animName")]
        public string AnimName { get; set; }

        [JsonProperty("animType")]
        public string AnimType { get; set; } //custom, static

        [JsonProperty("animData")]
        public string AnimData { get; set; }

        [JsonProperty("loop")]
        public bool Loop { get; set; }

        [JsonProperty("palette")]
        public List<Palette> Palette { get; set; }
    }
}

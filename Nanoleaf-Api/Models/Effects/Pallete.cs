using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Nanoleaf_Api.Models.Effects
{
    public class Pallete
    {
        [JsonProperty("hue")]
        public int Hue { get; set; }

        [JsonProperty("saturation")]
        public int Saturation { get; set; }

        [JsonProperty("brightness")]
        public int Brightness { get; set; }
    }
}

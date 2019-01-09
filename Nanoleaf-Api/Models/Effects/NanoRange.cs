using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Nanoleaf_Api.Models.Effects
{
    public class NanoRange
    {
        [JsonProperty("maxValue")]
        public int MaxValue { get; set; }

        [JsonProperty("minValue")]
        public int MinValue { get; set; }
    }
}

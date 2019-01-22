using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Nanoleaf_Models.Models.Sunset
{
    public class SunsetTimes
    {
        [JsonProperty("sunrise")]
        public DateTime Sunrise { get; set; }

        [JsonProperty("sunset")]
        public DateTime Sunset { get; set; }
    }
}

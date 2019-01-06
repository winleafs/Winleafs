using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Nanoleaf_Api.Models.State
{
    public class StateModel
    {
        [JsonProperty("state")]
        public int Value { get; set; }

        [JsonProperty("max")]
        public int Max { get; set; }

        [JsonProperty("min")]
        public int Min { get; set; }
    }
}
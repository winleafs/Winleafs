using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Nanoleaf_Models.Models.Layouts
{
    public class PanelPosition
    {
        [JsonProperty("panelId")]
        public int PanelId { get; set; }

        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("Y")]
        public int Y { get; set; }

        [JsonProperty("o")]
        public int Orientation { get; set; }
    }
}

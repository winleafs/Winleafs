using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Nanoleaf_Models.Models.Layouts
{
    public class Layout
    {
        [JsonProperty("numPanels")]
        public int NumberOfPanels { get; set; }

        [JsonProperty("sideLength")]
        public int SideLength { get; set; }

        [JsonProperty("positionData")]
        public IEnumerable<PanelPosition> PanelPositions { get; set; }
    }
}

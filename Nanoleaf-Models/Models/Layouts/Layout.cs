using System.Collections.Generic;

using Newtonsoft.Json;

namespace Winleafs.Models.Models.Layouts
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

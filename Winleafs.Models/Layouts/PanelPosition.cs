using Newtonsoft.Json;
using Winleafs.Models.Enums;

namespace Winleafs.Models.Layouts
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

        [JsonProperty("shapeType")]
        public ShapeType ShapeType { get; set; }
    }
}

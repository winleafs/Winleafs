using Newtonsoft.Json;

namespace Winleafs.Models.Models.Layouts
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

        /// <summary>
        /// To be sued when converting X to a local value  (e.g. pixels)
        /// </summary>
        public double TransformedX { get; set; }

        /// <summary>
        /// To be sued when converting Y to a local value  (e.g. pixels)
        /// </summary>
        public double TransformedY { get; set; }
    }
}

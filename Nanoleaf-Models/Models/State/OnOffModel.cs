using Newtonsoft.Json;

namespace Nanoleaf_Models.Models.State
{
    public class OnOffModel
    {
        [JsonProperty("value")]
        public bool IsTurnedOn { get; set; }
    }
}

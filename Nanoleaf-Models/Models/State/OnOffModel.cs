using Newtonsoft.Json;

namespace Winleafs.Models.Models.State
{
    public class OnOffModel
    {
        [JsonProperty("value")]
        public bool IsTurnedOn { get; set; }
    }
}

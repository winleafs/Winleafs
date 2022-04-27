using Newtonsoft.Json;

namespace Winleafs.Models.ExternalControl
{
    public class ExternalControlInfo
    {
        [JsonProperty("streamControlIpAddr")]
        public string StreamIPAddress { get; set; }

        [JsonProperty("streamControlPort")]
        public int StreamPort { get; set; }

        [JsonProperty("streamControlProtocol")]
        public string StreamIProtocol { get; set; }
    }
}

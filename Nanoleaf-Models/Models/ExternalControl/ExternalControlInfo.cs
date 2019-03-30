using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Winleafs.Models.Models.ExternalControl
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

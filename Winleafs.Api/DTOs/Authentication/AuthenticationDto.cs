using Newtonsoft.Json;

namespace Winleafs.Api.DTOs.Authentication
{
    public class AuthenticationDto
    {
        [JsonProperty("auth_token")]
        public string AuthenticationToken { get; set; }
    }
}

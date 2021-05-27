using Winleafs.Models.Models;

namespace Winleafs.Api.DTOs
{
    public class ClientDto
    {
        public ClientDto(string ip, int port, string authenticationToken = null)
        {
            Ip = ip;
            Port = port;
            AuthenticationToken = authenticationToken;
        }

        public ClientDto(Device device)
        {
            Ip = device.IPAddress;
            Port = device.Port;
            AuthenticationToken = device.AuthToken;
        }

        public string Ip { get; set; }

        public int Port { get; set; }

        public string AuthenticationToken { get; set; }

        public string BaseUrl => $"http://{Ip}:{Port}/api/v1/{AuthenticationToken}/";
    }
}

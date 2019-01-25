using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Nanoleaf_Api.Endpoints;
using Nanoleaf_Api.Endpoints.Interfaces;
using Nanoleaf_Models.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RestSharp;

namespace Nanoleaf_Api
{
    public interface INanoleafClient
    {
        IEffectsEndpoint EffectsEndpoint { get; }

        IAuthorizationEndpoint AuthorizationEndpoint { get; }

        ISunsetEndpoint SunsetEndpoint { get; }
    }

    public class NanoleafClient : INanoleafClient
    {
        private static Dictionary<string, INanoleafClient> _clients = new Dictionary<string, INanoleafClient>();

        internal Uri _baseUri;

        internal string _token = "";

        private IEffectsEndpoint _effectsEndpoint;

        public NanoleafClient(string ip, int port, string token = null)
        {
            _baseUri = new Uri($"http://{ip}:{port}");
            _token = token;
        }

        public static INanoleafClient GetClientForDevice(Device device)
        {
            if (!_clients.ContainsKey(device.IPAddress))
            {
                _clients.Add(device.IPAddress, new NanoleafClient(device.IPAddress, device.Port, device.AuthToken));
            }

            return _clients[device.IPAddress];
        }

        // Don't like this style and want to rework it.
        public IEffectsEndpoint EffectsEndpoint
        {
            get
            {
                if (_effectsEndpoint == null)
                {
                    _effectsEndpoint = new EffectsEndpoint(this);
                }

                return _effectsEndpoint;
            }
        }

        private IAuthorizationEndpoint _authorizationEndpoint;

        public IAuthorizationEndpoint AuthorizationEndpoint
        {
            get
            {
                if (_authorizationEndpoint == null)
                {
                    _authorizationEndpoint = new AuthorizationEndpoint(this);
                }

                return _authorizationEndpoint;
            }
        }

        public ISunsetEndpoint SunsetEndpoint => new SunsetEndpoint();
    }
}

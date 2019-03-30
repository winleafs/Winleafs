using System;
using System.Collections.Generic;

using Winleafs.Api.Endpoints;
using Winleafs.Api.Endpoints.Interfaces;
using Winleafs.Models.Models;

namespace Winleafs.Api
{
    public interface INanoleafClient
    {
        IEffectsEndpoint EffectsEndpoint { get; }

        IAuthorizationEndpoint AuthorizationEndpoint { get; }

        IStateEndpoint StateEndpoint { get; }
        IIdentifyEndpoint IdentifyEndpoint { get; }

        ILayoutEndpoint LayoutEndpoint { get; }
        IExternalControlEndpoint ExternalControlEndpoint { get; }

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

        public IEffectsEndpoint EffectsEndpoint => _effectsEndpoint ?? (_effectsEndpoint = new EffectsEndpoint(this));

        private IAuthorizationEndpoint _authorizationEndpoint;

        public IAuthorizationEndpoint AuthorizationEndpoint => _authorizationEndpoint ?? (_authorizationEndpoint = new AuthorizationEndpoint(this));

        private IStateEndpoint _stateEndpoint;

        public IStateEndpoint StateEndpoint => _stateEndpoint ?? (_stateEndpoint = new StateEndpoint(this));

        private ILayoutEndpoint _layoutEndpoint;

        public ILayoutEndpoint LayoutEndpoint => _layoutEndpoint ?? (_layoutEndpoint = new LayoutEndpoint(this));

        private IIdentifyEndpoint _identifyEndpoint;

        public IIdentifyEndpoint IdentifyEndpoint => _identifyEndpoint ?? (_identifyEndpoint = new IdentifyEndpoint(this));

        private IExternalControlEndpoint _externalControlEndpoint;

        public IExternalControlEndpoint ExternalControlEndpoint => _externalControlEndpoint ?? (_externalControlEndpoint = new ExternalControlEndpoint(this));
    }
}

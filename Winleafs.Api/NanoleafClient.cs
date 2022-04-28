using System;
using Winleafs.Nanoleaf.Endpoints;
using Winleafs.Nanoleaf.Endpoints.Interfaces;

namespace Winleafs.Nanoleaf
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
        /// <summary>
        /// Initializes a new instance of the <see cref="NanoleafClient"/> class.
        /// </summary>
        /// <param name="ip">The IP address at which the Nanoleaf device lives.</param>
        /// <param name="port">The port that is used to access the device.</param>
        /// <param name="token">The access token to access the device's API.</param>
        public NanoleafClient(string ip, int port, string token = null)
        {
            var connection = new NanoleafConnection(new Uri($"http://{ip}:{port}"), token);
            EffectsEndpoint = new EffectsEndpoint(connection);
            AuthorizationEndpoint = new AuthorizationEndpoint(connection);
            StateEndpoint = new StateEndpoint(connection);
            IdentifyEndpoint = new IdentifyEndpoint(connection);
            LayoutEndpoint = new LayoutEndpoint(connection);
            ExternalControlEndpoint = new ExternalControlEndpoint(connection);
        }

        public IEffectsEndpoint EffectsEndpoint { get; }

        public IAuthorizationEndpoint AuthorizationEndpoint { get; }

        public IStateEndpoint StateEndpoint { get; }

        public ILayoutEndpoint LayoutEndpoint { get; }

        public IIdentifyEndpoint IdentifyEndpoint { get; }

        public IExternalControlEndpoint ExternalControlEndpoint { get; }
    }
}

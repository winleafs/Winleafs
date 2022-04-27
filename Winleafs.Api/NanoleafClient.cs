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
        private readonly NanoleafConnection _connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="NanoleafClient"/> class.
        /// </summary>
        /// <param name="ip">The IP address at which the Nanoleaf device lives.</param>
        /// <param name="port">The port that is used to access the device.</param>
        /// <param name="token">The access token to access the device's API.</param>
        public NanoleafClient(string ip, int port, string token = null)
        {
            _connection = new NanoleafConnection(new Uri($"http://{ip}:{port}"), token);
        }

        public IEffectsEndpoint EffectsEndpoint => new EffectsEndpoint(_connection);

        public IAuthorizationEndpoint AuthorizationEndpoint => new AuthorizationEndpoint(_connection);

        public IStateEndpoint StateEndpoint => new StateEndpoint(_connection);

        public ILayoutEndpoint LayoutEndpoint => new LayoutEndpoint(_connection);

        public IIdentifyEndpoint IdentifyEndpoint => new IdentifyEndpoint(_connection);

        public IExternalControlEndpoint ExternalControlEndpoint => new ExternalControlEndpoint(_connection);
    }
}

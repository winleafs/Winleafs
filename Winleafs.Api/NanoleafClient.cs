using Winleafs.Api.DTOs;
using Winleafs.Api.Endpoints;
using Winleafs.Api.Endpoints.Interfaces;

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
        public NanoleafClient(ClientDto client)
        {
            EffectsEndpoint = new EffectsEndpoint(client);
            AuthorizationEndpoint = new AuthorizationEndpoint(client);
            StateEndpoint = new StateEndpoint(client);
            LayoutEndpoint = new LayoutEndpoint(client);
            IdentifyEndpoint = new IdentifyEndpoint(client);
            ExternalControlEndpoint = new ExternalControlEndpoint(client);
        }

        public IEffectsEndpoint EffectsEndpoint { get; }

        public IAuthorizationEndpoint AuthorizationEndpoint { get; }

        public IStateEndpoint StateEndpoint { get; }

        public ILayoutEndpoint LayoutEndpoint { get; }

        public IIdentifyEndpoint IdentifyEndpoint { get; }

        public IExternalControlEndpoint ExternalControlEndpoint { get; }
    }
}

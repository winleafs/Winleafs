using RestSharp;
using System.Threading.Tasks;
using Winleafs.Api.Endpoints.Interfaces;
using Winleafs.Models.Models.Layouts;

namespace Winleafs.Api.Endpoints
{
    public class LayoutEndpoint : NanoleafEndpoint, ILayoutEndpoint
    {
        /// <inheritdoc />
        public LayoutEndpoint(NanoleafClient client)
        {
            Client = client;
        }

        public GloabalOrientation GetGlobalOrientation()
        {
            return SendRequest<GloabalOrientation>("panelLayout/globalOrientation", Method.GET);
        }

        public async Task<GloabalOrientation> GetGlobalOrientationAsync()
        {
            return await SendRequestAsync<GloabalOrientation>("panelLayout/globalOrientation", Method.GET);
        }

        /// <inheritdoc />
        public Layout GetLayout()
        {
            return SendRequest<Layout>("panelLayout/layout", Method.GET);
        }

        /// <inheritdoc />
        public async Task<Layout> GetLayoutAsync()
        {
            return await SendRequestAsync<Layout>("panelLayout/layout", Method.GET).ConfigureAwait(false);
        }
    }
}

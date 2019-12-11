using RestSharp;
using System.Threading.Tasks;
using Winleafs.Api.Endpoints.Interfaces;
using Winleafs.Models.Models.Layouts;

namespace Winleafs.Api.Endpoints
{
    public class LayoutEndpoint : NanoleafEndpoint, ILayoutEndpoint
    {
        private const string BaseEndpoint = "panelLayout";

        /// <inheritdoc />
        public LayoutEndpoint(NanoleafClient client)
        {
            Client = client;
        }

        public GlobalOrientation GetGlobalOrientation()
        {
            return SendRequest<GlobalOrientation>($"{BaseEndpoint}/globalOrientation", Method.GET);
        }

        public async Task<GlobalOrientation> GetGlobalOrientationAsync()
        {
            return await SendRequestAsync<GlobalOrientation>($"{BaseEndpoint}/globalOrientation", Method.GET);
        }

        /// <inheritdoc />
        public Layout GetLayout()
        {
            return SendRequest<Layout>($"{BaseEndpoint}/layout", Method.GET);
        }

        /// <inheritdoc />
        public async Task<Layout> GetLayoutAsync()
        {
            return await SendRequestAsync<Layout>($"{BaseEndpoint}/layout", Method.GET).ConfigureAwait(false);
        }
    }
}

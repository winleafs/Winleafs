using System.Threading.Tasks;
using RestSharp;
using Winleafs.Nanoleaf.Endpoints.Interfaces;

namespace Winleafs.Nanoleaf.Endpoints
{
    public class IdentifyEndpoint : NanoleafEndpoint, IIdentifyEndpoint
    {
        /// <inheritdoc />
        public IdentifyEndpoint(NanoleafClient client)
        {
            Client = client;
        }

        /// <inheritdoc />
        public Task Identify()
        {
            return SendRequestAsync("identify", Method.PUT);
        }
    }
}

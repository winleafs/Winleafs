using System.Threading.Tasks;

using RestSharp;

using Winleafs.Api.Endpoints.Interfaces;

namespace Winleafs.Api.Endpoints
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
            return SendRequest("identify", Method.PUT);
        }
    }
}

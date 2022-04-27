using System.Threading.Tasks;
using RestSharp;
using Winleafs.Nanoleaf.Endpoints.Interfaces;

namespace Winleafs.Nanoleaf.Endpoints
{
    public class IdentifyEndpoint : NanoleafEndpoint, IIdentifyEndpoint
    {
        public IdentifyEndpoint(NanoleafConnection connection) : base(connection)
        {
        }

        /// <inheritdoc />
        public Task Identify()
        {
            return SendRequestAsync("identify", Method.PUT);
        }
    }
}

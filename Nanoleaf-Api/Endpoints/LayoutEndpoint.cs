using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
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

        /// <inheritdoc />
        public void Identify()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task IdentifyAsync()
        {
            throw new NotImplementedException();
        }
    }
}

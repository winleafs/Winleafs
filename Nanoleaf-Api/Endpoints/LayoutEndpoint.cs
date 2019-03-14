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
            return GetLayoutAsync().GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public Task<Layout> GetLayoutAsync()
        {
            return SendRequest<Layout>("panelLayout/layout", Method.GET);
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

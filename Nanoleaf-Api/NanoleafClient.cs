using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Nanoleaf_Api.Endpoints;
using Nanoleaf_Api.Endpoints.Interfaces;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RestSharp;

namespace Nanoleaf_Api
{
    public interface INanoleafClient
    {
        /// <summary>
        /// Authorizes with the Nanoleaf device.
        /// </summary>
        Task AuthorizeAsync(string ip, int port);

        IEffectsEndpoint EffectsEndpoint { get; }
    }

    public class NanoleafClient : INanoleafClient
    {
        internal Uri _baseUri;

        internal string _token = "";

        public async Task AuthorizeAsync(string ip, int port)
        {
            // Rework to own endpoint.
            _baseUri = new Uri($"http://{ip}:{port}");
            var client = new RestClient(_baseUri);
            var request = new RestRequest("api/v1/new", Method.POST);
            var response = await client.ExecuteTaskAsync(request);
            var jObject = JObject.Parse(response.Content);
            _token = jObject["auth_token"].ToString();
        }

        private IEffectsEndpoint _effectsEndpoint;

        // Don't like this style and want to rework it.
        public IEffectsEndpoint EffectsEndpoint
        {
            get
            {
                if (_effectsEndpoint == null)
                {
                    _effectsEndpoint = new EffectsEndpoint(this);
                }

                return _effectsEndpoint;
            }
        }
    }
}

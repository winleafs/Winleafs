using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using RestSharp;

namespace Nanoleaf_Api.Endpoints
{
    /// <summary>
    /// A class used to easily send requests to the Nanoleaf device.
    /// </summary>
    public abstract class NanoleafEndpoint
    {
        protected NanoleafClient Client { get; set; }

        /// <summary>
        /// Sends a request to the Nanoleaf.
        /// </summary>
        /// <typeparam name="T">The type of response wanting to be gotten.</typeparam>
        /// <param name="endpoint">The endpoint where the requests needs to be send to.</param>
        /// <param name="method">The method that should be used.</param>
        /// <param name="body">Optionally the body which should be provided.</param>
        /// <returns>An awaitable task containing the wanted result.</returns>
        protected async Task<T> SendRequest<T>(string endpoint, Method method, object body = null)
        {
            return (T)await SendRequest(endpoint, method, typeof(T), body);
        }

        /// <summary>
        /// Sends a request to the Nanoleaf.
        /// </summary>
        /// <param name="endpoint">The endpoint where the requests needs to be send to.</param>
        /// <param name="method">The method that should be used.</param>
        /// <param name="returnType">The type which should be return. If null is provided null will be returned.</param>
        /// <param name="body">Optionally the body which should be provided.</param>
        /// <returns>An awaitable task containing the wanted result.</returns>
        protected async Task<object> SendRequest(string endpoint, Method method, Type returnType = null, object body = null)
        {
            var restClient = new RestClient(Client._baseUri);
            var request = new RestRequest($"api/v1/{Client._token}{endpoint}", method);
            if (body != null)
            {
                request.AddJsonBody(body);
            }

            var response = await restClient.ExecuteTaskAsync(request);

            return returnType == null ? null : JsonConvert.DeserializeObject(response.Content, returnType);
        }
    }
}

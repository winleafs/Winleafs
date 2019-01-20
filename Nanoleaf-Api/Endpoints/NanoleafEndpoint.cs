using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using RestSharp;

namespace Nanoleaf_Api.Endpoints
{
    public abstract class NanoleafEndpoint
    {
        protected NanoleafClient Client { get; set; }

        protected async Task<T> SendRequest<T>(string endpoint, Method method, object body = null)
        {
            return (T)await SendRequest(endpoint, method, typeof(T), body);
        }

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

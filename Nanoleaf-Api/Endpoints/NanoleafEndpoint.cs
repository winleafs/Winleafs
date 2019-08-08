using System;
using System.Threading.Tasks;

using Newtonsoft.Json;

using NLog;

using RestSharp;

namespace Winleafs.Api.Endpoints
{
    /// <summary>
    /// A class used to easily send requests to the Nanoleaf device.
    /// </summary>
    public abstract class NanoleafEndpoint
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        protected NanoleafClient Client { get; set; }

        /// <summary>
        /// Sends a request to the Nanoleaf.
        /// </summary>
        /// <typeparam name="T">The type of response wanting to be gotten.</typeparam>
        /// <param name="endpoint">The endpoint where the requests needs to be send to.</param>
        /// <param name="method">The method that should be used.</param>
        /// <param name="body">Optionally the body which should be provided.</param>
        /// <returns>An awaitable task containing the wanted result.</returns>
        protected async Task<T> SendRequestAsync<T>(string endpoint, Method method, object body = null)
        {
            return (T)await SendRequestAsync(endpoint, method, typeof(T), body);
        }

        /// <summary>
        /// Sends a request to the Nanoleaf.
        /// </summary>
        /// <typeparam name="T">The type of response wanting to be gotten.</typeparam>
        /// <param name="endpoint">The endpoint where the requests needs to be send to.</param>
        /// <param name="method">The method that should be used.</param>
        /// <param name="body">Optionally the body which should be provided.</param>
        /// <returns>The wanted result.</returns>
        protected T SendRequest<T>(string endpoint, Method method, object body = null)
        {
            return (T)SendRequest(endpoint, method, typeof(T), body);
        }

        /// <summary>
        /// Sends a request to the Nanoleaf.
        /// </summary>
        /// <param name="endpoint">The endpoint where the requests needs to be send to.</param>
        /// <param name="method">The method that should be used.</param>
        /// <param name="returnType">The type which should be return. If null is provided null will be returned.</param>
        /// <param name="body">Optionally the body which should be provided.</param>
        /// <param name="disableLogging">Disables the logging when set to true.</param>
        /// <returns>An awaitable task containing the wanted result.</returns>
        protected async Task<object> SendRequestAsync(string endpoint, Method method, Type returnType = null, object body = null, bool disableLogging = false)
        {
            var restClient = new RestClient(Client._baseUri);
            var request = new RestRequest($"api/v1/{Client._token}/{endpoint}", method);
            if (body != null)
            {
                request.AddJsonBody(body);
            }

            if (!disableLogging)
            {
                _logger.Info(
                    $"Sending following request: Address: {Client._baseUri}, URL: {request.Resource}, Method: {method.ToString()}, Body: {(body != null ? body.ToString() : "")}");
            }

            var response = await restClient.ExecuteTaskAsync(request).ConfigureAwait(false);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                _logger.Warn($"Request failed, statuscode: {(int)response.StatusCode} {response.StatusCode.ToString()}, status description: {response.StatusDescription}, content: {response.Content}");
            }

            return returnType == null ? null : JsonConvert.DeserializeObject(response.Content, returnType);
        }

        /// <summary>
        /// Sends a request to the Nanoleaf.
        /// </summary>
        /// <param name="endpoint">The endpoint where the requests needs to be send to.</param>
        /// <param name="method">The method that should be used.</param>
        /// <param name="returnType">The type which should be return. If null is provided null will be returned.</param>
        /// <param name="body">Optionally the body which should be provided.</param>
        /// <param name="disableLogging">Disables the logging when set to true.</param>
        /// <returns>The wanted result.</returns>
        protected object SendRequest(string endpoint, Method method, Type returnType = null, object body = null, bool disableLogging = false)
        {
            var restClient = new RestClient(Client._baseUri);
            var request = new RestRequest($"api/v1/{Client._token}/{endpoint}", method);
            if (body != null)
            {
                request.AddJsonBody(body);
            }

            if (!disableLogging)
            {
                _logger.Info(
                    $"Sending following request: Address: {Client._baseUri}, URL: {request.Resource}, Method: {method.ToString()}, Body: {(body != null ? body.ToString() : "")}");
            }

            var response = restClient.Execute(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                _logger.Warn($"Request failed, statuscode: {(int)response.StatusCode} {response.StatusCode.ToString()}, status description: {response.StatusDescription}, content: {response.Content}");
            }

            return returnType == null ? null : JsonConvert.DeserializeObject(response.Content, returnType);
        }
    }
}

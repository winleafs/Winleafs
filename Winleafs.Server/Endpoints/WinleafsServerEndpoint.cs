using Newtonsoft.Json;
using NLog;
using RestSharp;
using System;
using System.Net;
using System.Threading.Tasks;
using Winleafs.Server.Exceptions;

namespace Winleafs.Server.Endpoints
{
    public abstract class WinleafsServerEndpoint
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        protected WinleafsServerClient Client { get; set; }

        // 2 Second default timeout.
        protected int Timeout { get; set; } = 2000;

        /// <summary>
        /// Sends a request to the Winleafs server.
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
        /// Sends a request to the Winleafs server.
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
        /// Sends a request to the Winleafs server.
        /// </summary>
        /// <param name="endpoint">The endpoint where the requests needs to be send to.</param>
        /// <param name="method">The method that should be used.</param>
        /// <param name="returnType">
        /// The type which should be return. If null is provided null will be returned.
        /// </param>
        /// <param name="body">Optionally the body which should be provided.</param>
        /// <param name="disableLogging">Disables the logging when set to true.</param>
        /// <returns>An awaitable task containing the wanted result.</returns>
        protected async Task<object> SendRequestAsync(string endpoint, Method method,
            Type returnType = null, object body = null, bool disableLogging = false)
        {
            var restClient = new RestClient(Client.BaseUri);
            var request = new RestRequest($"/{endpoint}", method)
            {
                Timeout = Timeout
            };

            if (body != null)
            {
                request.AddJsonBody(body);
            }

            if (!disableLogging)
            {
                LogRequest(request, method, body);
            }

            var response = await restClient.ExecuteTaskAsync(request).ConfigureAwait(false);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                LogError(response);
                throw new WinleafsServerException("Error during request to Winleafs server");
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
            var restClient = new RestClient(Client.BaseUri);
            var request = new RestRequest($"/{endpoint}", method)
            {
                Timeout = Timeout //Set timeout to 2 seconds
            };

            if (body != null)
            {
                request.AddJsonBody(body);
            }

            if (!disableLogging)
            {
                LogRequest(request, method, body);
            }

            var response = restClient.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                LogError(response);
                throw new WinleafsServerException("Error during request to Winleafs server");
            }

            return returnType == null ? null : JsonConvert.DeserializeObject(response.Content, returnType);
        }

        protected void LogRequest(RestRequest request, Method method, object body)
        {
            _logger.Info(
                $"Sending following request Winleafs Server: Address: {Client.BaseUri}, " +
                $"URL: {request.Resource}, Method: {method.ToString()}, " +
                $"Body: {(body != null ? body.ToString() : "")}");
        }

        protected void LogError(IRestResponse response)
        {
            _logger.Warn($"Winleafs server request failed, statuscode: {(int)response.StatusCode} " +
                         $"{response.StatusCode.ToString()}, status description: " +
                         $"{response.StatusDescription}, content: {response.Content}");
        }
    }
}

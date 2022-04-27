using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using RestSharp;

namespace Winleafs.Nanoleaf.Endpoints
{
    /// <summary>
    /// A class used to easily send requests to the Nanoleaf device.
    /// </summary>
    public abstract class NanoleafEndpoint
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected NanoleafConnection Connection { get; }

        protected NanoleafEndpoint(NanoleafConnection connection)
        {
            Connection = connection;
        }

        // 2 Second default timeout.
        protected int Timeout { get; set; } = 2000;

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
        /// <returns>The wanted result.</returns>
        protected object SendRequest(string endpoint, Method method, Type returnType = null, object body = null, bool disableLogging = false)
        {
            return SendRequestAsync(endpoint, method, returnType, body, disableLogging).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Sends a request to the Nanoleaf.
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
            var restClient = new RestClient(Connection.Uri);
            var request = new RestRequest(GetUrlForRequest(endpoint), method)
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

            var response = await restClient.ExecuteAsync(request).ConfigureAwait(false);

            if (!response.IsSuccessful)
            {
                LogError(response);
            }

            if (returnType == null || string.IsNullOrWhiteSpace(response.Content))
            {
                return null;
            }

            return JsonConvert.DeserializeObject(response.Content, returnType);
        }

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

        protected string GetUrlForRequest(string endpoint)
        {
            return $"api/v1/{Connection.Token}/{endpoint}";
        }

        protected void LogRequest(RestRequest request, Method method, object body)
        {
            _logger.Info(
                $"Sending following request to Nanoleaf: Address: {Connection.Uri}, " +
                $"URL: {request.Resource}, Method: {method}, " +
                $"Body: {(body != null ? body.ToString() : "")}");
        }

        protected void LogError(IRestResponse response)
        {
            _logger.Warn($"Nanoleaf request failed, statuscode: {(int)response.StatusCode} " +
                         $"{response.StatusCode}, status description: " +
                         $"{response.StatusDescription}, content: {response.Content}");
        }
    }
}

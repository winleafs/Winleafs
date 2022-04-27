using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;
using Winleafs.Nanoleaf.Endpoints.Interfaces;

namespace Winleafs.Nanoleaf.Endpoints
{
    public class AuthorizationEndpoint : NanoleafEndpoint, IAuthorizationEndpoint
    {
        /// <inheritdoc />
        public AuthorizationEndpoint(NanoleafClient client)
        {
            Client = client;
        }

        /// <inheritdoc />
        public string GetAuthToken()
		{
			return GetAuthTokenAsync().ConfigureAwait(false).GetAwaiter().GetResult();
		}

        /// <inheritdoc />
        public async Task<string> GetAuthTokenAsync()
        {
            var client = new RestClient(Client.BaseUri);
            var request = new RestRequest("api/v1/new", Method.POST);
            var response = await client.ExecuteAsync(request).ConfigureAwait(false);

            var jObject = JObject.Parse(response.Content);
            Client.Token = jObject["auth_token"].ToString();

            return Client.Token;
        }
    }
}

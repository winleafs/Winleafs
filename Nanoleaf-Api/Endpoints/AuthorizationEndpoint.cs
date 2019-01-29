using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using RestSharp;

using Winleafs.Api.Endpoints.Interfaces;

namespace Winleafs.Api.Endpoints
{
    public class AuthorizationEndpoint : NanoleafEndpoint, IAuthorizationEndpoint
    {
        public AuthorizationEndpoint(NanoleafClient client)
        {
            Client = client;
        }

        public async Task<string> GetAuthToken()
        {
            var client = new RestClient(Client._baseUri);
            var request = new RestRequest("api/v1/new", Method.POST);
            var response = await client.ExecuteTaskAsync(request);

            var jObject = JObject.Parse(response.Content);
            Client._token = jObject["auth_token"].ToString();

            // TODO Save token somewhere.
            return Client._token;
        }
    }
}

using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using Winleafs.Api.DTOs;
using Winleafs.Api.DTOs.Authentication;
using Winleafs.Api.Endpoints.Interfaces;

namespace Winleafs.Api.Endpoints
{
	public class AuthorizationEndpoint : NanoleafEndpoint, IAuthorizationEndpoint
	{
		/// <inheritdoc />
		public AuthorizationEndpoint(ClientDto client)
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
			var client = new RestClient($"http://{Client.Ip}:{Client.Port}");
			client.UseNewtonsoftJson();
			var request = new RestRequest("api/v1/new", Method.POST);
			var response = await client.ExecuteAsync<AuthenticationDto>(request).ConfigureAwait(false);


			if (!response.IsSuccessful)
			{
				return null;
			}

			Client.AuthenticationToken = response.Data.AuthenticationToken;

			return Client.AuthenticationToken;
		}
	}
}

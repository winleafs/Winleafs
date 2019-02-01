using System;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using RestSharp;

using Winleafs.Api.Endpoints.Interfaces;
using Winleafs.Models.Models.Sunset;

namespace Winleafs.Api.Endpoints
{
    public class SunsetEndpoint : ISunsetEndpoint
    {
        public async Task<SunsetTimes> GetSunsetSunriseAsync(double lat, double lon)
        {
            var client = new RestClient("https://api.sunrise-sunset.org");
            var request = new RestRequest($"/json?lat={Format(lat)}&lng={Format(lon)}", Method.GET);
            var response = await client.ExecuteTaskAsync(request).ConfigureAwait(false);
            var jsonObject = JObject.Parse(response.Content);
            return jsonObject["results"].ToObject<SunsetTimes>();
        }

        /// <summary>
        /// Formats the number to be 7 digits as the API wants.
        /// </summary>
        /// <param name="number">The number wanting to be converted.</param>
        /// <returns>The number with only 7 digits.</returns>
        private static string Format(double number)
        {
            return Math.Round(number, 7).ToString("N7");
        }

		public SunsetTimes GetSunsetSunrise(double lat, double lon)
		{
			return GetSunsetSunriseAsync(lat, lon).ConfigureAwait(false).GetAwaiter().GetResult();
		}
	}
}

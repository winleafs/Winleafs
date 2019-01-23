using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Nanoleaf_Api.Endpoints.Interfaces;

using Nanoleaf_Models.Models.Sunset;

using Newtonsoft.Json.Linq;

using RestSharp;

namespace Nanoleaf_Api.Endpoints
{
    public class SunsetEndpoint : ISunsetEndpoint
    {
        public async Task<SunsetTimes> GetSunsetSunrise(double lat, double lon)
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
    }
}

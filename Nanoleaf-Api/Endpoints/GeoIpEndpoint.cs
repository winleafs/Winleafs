using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using Winleafs.Api.Endpoints.Interfaces;
using Winleafs.Models.Models.GeoIp;

namespace Winleafs.Api.Endpoints
{
    public class GeoIpEndpoint : IGeoIpEndpoint
    {
        private const string IPAPI = "http://icanhazip.com";
        private static readonly Uri GeopIpUri = new Uri("http://ip-api.com/");
        
        public GeoIpResult GetGeoIpData()
        {
            var ip = GetIp();
            var client = new RestClient(GeopIpUri);
            var request = new RestRequest($"json/{ip}");
            var response = client.Execute(request, Method.GET);
            if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<GeoIpResult>(response.Content);
        }

        public async Task<GeoIpResult> GetGeoIpDataAsync()
        {
            var ip = await GetIpASync();
            var client = new RestClient(GeopIpUri);
            var request = new RestRequest($"json/{ip}");
            var response = await client.ExecuteTaskAsync(request);
            if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<GeoIpResult>(response.Content);
        }

        private string GetIp()
        {
            return new WebClient().DownloadString(IPAPI);
        }

        private Task<string> GetIpASync()
        {
            return new WebClient().DownloadStringTaskAsync(IPAPI);
        }
    }
}
using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using Winleafs.External.interfaces;
using Winleafs.Models.Models.GeoIp;

namespace Winleafs.External
{
    public class GeoIpClient : IGeoIpClient
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
            var response = await client.ExecuteAsync(request);
            if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<GeoIpResult>(response.Content);
        }

        private static string GetIp()
        {
            return new WebClient().DownloadString(IPAPI);
        }

        private static Task<string> GetIpASync()
        {
            return new WebClient().DownloadStringTaskAsync(IPAPI);
        }
    }
}
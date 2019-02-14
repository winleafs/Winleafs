using System.Threading.Tasks;
using Winleafs.Models.Models.GeoIp;

namespace Winleafs.External.interfaces
{
    public interface IGeoIpClient
    {
        /// <inheritdoc cref="GetGeoIpDataAsync"/>
        GeoIpResult GetGeoIpData();

        /// <summary>
        /// Gets the Geo-IP data based off the user's IP.
        /// </summary>
        /// <returns>The GeoIp data.</returns>
        Task<GeoIpResult> GetGeoIpDataAsync();
    }
}
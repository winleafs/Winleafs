using System.Threading.Tasks;

using Winleafs.Models.Models.Sunset;

namespace Winleafs.Api.Endpoints.Interfaces
{
    public interface ISunsetEndpoint
    {
        Task<SunsetTimes> GetSunsetSunrise(double lat, double lon);
    }
}

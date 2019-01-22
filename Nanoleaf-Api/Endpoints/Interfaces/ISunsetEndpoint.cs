using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Nanoleaf_Models.Models.Sunset;

namespace Nanoleaf_Api.Endpoints.Interfaces
{
    public interface ISunsetEndpoint
    {
        Task<SunsetTimes> GetSunsetSunrise(double lat, double lon);
    }
}

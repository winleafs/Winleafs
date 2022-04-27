using System;
using Newtonsoft.Json;

namespace Winleafs.Models.Sunset
{
    public class SunsetTimes
    {
        [JsonProperty("sunrise")]
        public DateTime Sunrise { get; set; }

        [JsonProperty("sunset")]
        public DateTime Sunset { get; set; }

        public int SunsetHour => Sunset.Hour;

        public int SunsetMinute => Sunset.Minute;

        public int SunriseHour => Sunrise.Hour;

        public int SunriseMinute => Sunrise.Minute;
    }
}

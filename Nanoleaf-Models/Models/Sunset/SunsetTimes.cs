using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Nanoleaf_Models.Models.Sunset
{
    public class SunsetTimes
    {
        [JsonProperty("sunrise")]
        public DateTime Sunrise { get; set; }

        [JsonProperty("sunset")]
        public DateTime Sunset { get; set; }

        public int SunsetHour
        {
            get
            {
                return Sunset.Hour;
            }
        }

        public int SunsetMinute
        {
            get
            {
                return Sunset.Minute;
            }
        }

        public int SunriseHour
        {
            get
            {
                return Sunrise.Hour;
            }
        }

        public int SunriseMinute
        {
            get
            {
                return Sunrise.Minute;
            }
        }
    }
}

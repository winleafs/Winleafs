using Nanoleaf_Api.Endpoints;
using Nanoleaf_Models.Models;
using Polly;
using System;
using System.Threading.Tasks;

namespace Nanoleaf_Api.Timers
{
    public class SunTimesUpdater
    {
        public static void UpdateSunTimes()
        {
            if (UserSettings.Settings.Latitude.HasValue && UserSettings.Settings.Longitude.HasValue)
            {
                var policy = Policy
                    .Handle<Exception>()
                    .WaitAndRetry(5, retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                    );

                //TODO: add logging on failure, see Step 2 in https://github.com/App-vNext/Polly

                policy.Execute(() => RequestSunTimes());
            }            
        }

        private static async Task RequestSunTimes()
        {
            var sunApi = new SunsetEndpoint();
            var sunTimes = await sunApi.GetSunsetSunrise(UserSettings.Settings.Latitude.Value, UserSettings.Settings.Longitude.Value);

            UserSettings.Settings.UpdateSunriseSunset(sunTimes.SunriseHour, sunTimes.SunriseMinute, sunTimes.SunsetHour, sunTimes.SunsetMinute);
        }
    }
}

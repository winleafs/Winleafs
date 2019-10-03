using System;
using System.Threading.Tasks;

using Polly;

using Winleafs.Api.Endpoints;
using Winleafs.External;
using Winleafs.Models.Models;

namespace Winleafs.Api.Helpers
{
    public static class SunTimesUpdater
    {
        /// <summary>
        /// Pulls the Sunrise and Sunset hours and minutes from the API
        /// Only if the user has the <see cref="UserSettings.Settings.Latitude"/>
        /// and <see cref="UserSettings.Settings.Longitude"/> set
        /// </summary>
        public static void UpdateSunTimes()
        {
            if (UserSettings.Settings.Latitude.HasValue && UserSettings.Settings.Longitude.HasValue)
            {
                var policy = Policy
                    .Handle<Exception>()
                    .WaitAndRetry(
                        5,
                        retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

                //TODO: add logging on failure, see Step 2 in https://github.com/App-vNext/Polly

                policy.Execute(() => RequestSunTimes());
            }
        }

        private static async Task RequestSunTimes()
        {
            var client = new SunsetSunriseClient();
            var sunTimes = await client.GetSunsetSunriseAsync(UserSettings.Settings.Latitude.Value, UserSettings.Settings.Longitude.Value);

            UserSettings.Settings.UpdateSunriseSunset(sunTimes.SunriseHour, sunTimes.SunriseMinute, sunTimes.SunsetHour, sunTimes.SunsetMinute);
        }
    }
}

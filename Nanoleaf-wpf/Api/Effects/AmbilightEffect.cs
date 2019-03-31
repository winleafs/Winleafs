using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Winleafs.Api;
using Winleafs.Models.Models;
using Winleafs.Wpf.Helpers;

namespace Winleafs.Wpf.Api.Effects
{
    public class AmbilightEffect : ICustomEffect
    {
        private readonly INanoleafClient _nanoleafClient;
        private readonly System.Timers.Timer _timer;
        private readonly bool _controlBrightness;

        public AmbilightEffect(INanoleafClient nanoleafClient)
        {
            _nanoleafClient = nanoleafClient;

            _controlBrightness = UserSettings.Settings.AmbilightControlBrightness;

            var timerRefreshRate = 1000;

            if (UserSettings.Settings.AmbilightRefreshRatePerSecond > 0 && UserSettings.Settings.AmbilightRefreshRatePerSecond <= 10)
            {
                timerRefreshRate = 1000 / UserSettings.Settings.AmbilightRefreshRatePerSecond;
            }

            if (_controlBrightness && timerRefreshRate < 1000 / 5)
            {
                timerRefreshRate = 1000 / 5; //When this effect also control brightness, we can update a maximum of 5 times per second since setting brightness is a different action
            }

            _timer = new System.Timers.Timer(timerRefreshRate);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Task.Run(() => SetColor());
        }

        private async Task SetColor()
        {
            var color = ScreenGrabber.GetColor();

            var hue = (int)color.GetHue();
            var sat = (int)(color.GetSaturation() * 100);

            // Sets the color of the nanoleaf with the logging disabled.
            // Seeing as a maximum of 10 requests per second can be set this will generate a lot of unwanted log data.
            // See https://github.com/StijnOostdam/Winleafs/issues/40.
            if (_controlBrightness)
            {
                //For brightness calculation see: https://stackoverflow.com/a/596243 and https://www.w3.org/TR/AERT/#color-contrast
                //We do not use Color.GetBrightness() since that value is always null because we use Color.FromArgb in the screengrabber.
                //Birghtness can be maximum 100
                var brightness = Math.Min(100, (int)(0.299 * color.R + 0.587 * color.G + 0.114 * color.B));

                await _nanoleafClient.StateEndpoint.SetHueSaturationAndBrightnessAsync(hue, sat, brightness, disableLogging: true);
            }
            else
            {
                await _nanoleafClient.StateEndpoint.SetHueAndSaturationAsync(hue, sat, disableLogging: true);
            }
        }

        public string GetName()
        {
            return $"{CustomEffectsCollection.EffectNamePreface}Ambilight";
        }

        public async Task Activate()
        {
            ScreenGrabber.Start();
            _timer.Start();
        }

        public async Task Deactivate()
        {
            _timer.Stop();
            Thread.Sleep(1000); //Give the last command the time to complete, 1000 is based on testing and a high value (better safe then sorry)

            var ambilightActive = false;
            var name = GetName();

            foreach (var device in UserSettings.Settings.Devices)
            {
                ambilightActive = name.Equals(device.GetActiveEffect());
            }

            if (!ambilightActive)
            {
                ScreenGrabber.Stop();
            }
        }

        public bool IsContinuous()
        {
            return true;
        }

        public bool IsActive()
        {
            return _timer.Enabled;
        }
    }
}
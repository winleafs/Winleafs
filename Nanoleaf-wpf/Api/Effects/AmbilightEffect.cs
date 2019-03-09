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
        private INanoleafClient _nanoleafClient;
        private System.Timers.Timer _timer;

        public AmbilightEffect(INanoleafClient nanoleafClient)
        {
            _nanoleafClient = nanoleafClient;

            var timerRefreshRate = 1000;

            if (UserSettings.Settings.AmbilightRefreshRatePerSecond > 0 && UserSettings.Settings.AmbilightRefreshRatePerSecond <= 10)
            {
                timerRefreshRate = 1000 / UserSettings.Settings.AmbilightRefreshRatePerSecond;
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

            if (color != null)
            {
                var hue = (int)color.GetHue();
                var sat = (int)(color.GetSaturation() * 100);

                // Sets the color of the nanoleaf with the logging disabled.
                // Seeing as a maximum of 10 requests per second can be set this will generate a lot of unwanted log data.
                // See https://github.com/StijnOostdam/Winleafs/issues/40.
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

            //TODO: check if the screengrabber can stop
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
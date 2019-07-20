using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Winleafs.Api;
using Winleafs.Models.Models;
using Winleafs.Wpf.Helpers;

namespace Winleafs.Wpf.Api.Effects
{
    public class ScreenMirrorEffect : ICustomEffect
    {
        public static string Name => $"{CustomEffectsCollection.EffectNamePreface}Screen mirror";

        private readonly INanoleafClient _nanoleafClient;
        private readonly System.Timers.Timer _timer;

        public ScreenMirrorEffect(INanoleafClient nanoleafClient)
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

        /// <summary>
        /// Sets the color of the nanoleaf with the logging disabled.
        /// Seeing as a maximum of 10 requests per second can be set this will generate a lot of unwanted log data.
        /// See https://github.com/StijnOostdam/Winleafs/issues/40.
        /// </summary>
        private async Task SetColor()
        {
            var color = ScreenGrabber.GetColor();

            var hue = (int)color.GetHue();
            var sat = (int)(color.GetSaturation() * 100);
            
            await _nanoleafClient.StateEndpoint.SetHueAndSaturationAsync(hue, sat, disableLogging: true);
        }

        public async Task Activate()
        {
            ScreenGrabber.StartScreenMirror();
            _timer.Start();
        }

        /// <summary>
        /// Stops the timer and gives it 1 second to complete. Also stop the screengrabber if no other ambilight effects are active
        /// </summary>
        public async Task Deactivate()
        {
            _timer.Stop();
            Thread.Sleep(1000); //Give the last command the time to complete, 1000 is based on testing and a high value (better safe then sorry)

            var screenMirrorActive = false;

            foreach (var device in UserSettings.Settings.Devices)
            {
                screenMirrorActive = Name.Equals(OrchestratorCollection.GetOrchestratorForDevice(device).GetActiveEffectName());
            }

            if (!screenMirrorActive)
            {
                ScreenGrabber.StopScreenMirror();
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
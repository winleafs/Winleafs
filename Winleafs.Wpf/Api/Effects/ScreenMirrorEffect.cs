using NLog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Media;
using Winleafs.Api;
using Winleafs.Models.Enums;
using Winleafs.Models.Models;
using Winleafs.Wpf.Api.Effects.ScreenMirrorEffects;
using Winleafs.Wpf.Api.Layouts;

namespace Winleafs.Wpf.Api.Effects
{
    public class ScreenMirrorEffect : ICustomEffect
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        private readonly INanoleafClient _nanoleafClient;
        private readonly System.Timers.Timer _timer;
        private readonly ScreenMirrorAlgorithm _screenMirrorAlgorithm;
        private readonly IScreenMirrorEffect _screenMirrorEffect;

        public ScreenMirrorEffect(Device device, Orchestrator orchestrator, INanoleafClient nanoleafClient)
        {
            _nanoleafClient = nanoleafClient;
            _screenMirrorAlgorithm = device.ScreenMirrorAlgorithm;

            try
            {
                if (_screenMirrorAlgorithm == ScreenMirrorAlgorithm.ScreenMirrorFit)
                {
                    _screenMirrorEffect = new ScreenMirror(device, orchestrator, nanoleafClient, ScaleType.Fit);
                }
                else if (_screenMirrorAlgorithm == ScreenMirrorAlgorithm.ScreenMirrorStretch)
                {
                    _screenMirrorEffect = new ScreenMirror(device, orchestrator, nanoleafClient, ScaleType.Stretch);
                }
                else if (_screenMirrorAlgorithm == ScreenMirrorAlgorithm.Ambilight)
                {
                    _screenMirrorEffect = new Ambilght(nanoleafClient, device);
                }
            }
            catch (Exception e)
            {
                // It is possible that the user adjusted his/her screens and therefore we get an error when initializing the effect
                _logger.Error(e, $"Something went wrong initializing the screen mirror effect for device {device.ToString()}");
                _screenMirrorEffect = null;
            }

            var timerRefreshRate = 1000;

            if (UserSettings.Settings.ScreenMirrorRefreshRatePerSecond > 0 && UserSettings.Settings.ScreenMirrorRefreshRatePerSecond <= 10)
            {
                timerRefreshRate = 1000 / UserSettings.Settings.ScreenMirrorRefreshRatePerSecond;
            }

            _timer = new System.Timers.Timer(timerRefreshRate);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Task.Run(() => ApplyEffect());
        }

        private async Task ApplyEffect()
        {
            if (_screenMirrorEffect != null)
            {
                await _screenMirrorEffect.ApplyEffect();
            }            
        }

        /// <inheritdoc />
        public async Task Activate()
        {
            if (_screenMirrorAlgorithm == ScreenMirrorAlgorithm.ScreenMirrorFit || _screenMirrorAlgorithm == ScreenMirrorAlgorithm.ScreenMirrorStretch)
            {
                //For screen mirror, we need to enable external control
                await _nanoleafClient.ExternalControlEndpoint.PrepareForExternalControl();
            }

            _timer.Start();
        }

        /// <summary>
        /// Stops the timer and gives it 1 second to complete. Also stop the screengrabber if no other ambilight effects are active
        /// </summary>
        public async Task Deactivate()
        {
            _timer.Stop();
            Thread.Sleep(1000); //Give the last command the time to complete, 1000 is based on testing and a high value (better safe then sorry)
        }

        /// <inheritdoc />
        public bool IsContinuous()
        {
            return true;
        }

        /// <inheritdoc />
        public bool IsActive()
        {
            return _timer.Enabled;
        }

        /// <inheritdoc />
        public List<System.Drawing.Color> GetColors()
        {
            return new List<System.Drawing.Color> { ICustomEffect.DefaultColor };
        }

        public string GetName()
        {
            return $"{UserSettings.EffectNamePreface}Screen mirror";
        }
    }
}
using NLog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Winleafs.Models;
using Winleafs.Models.Enums;
using Winleafs.Nanoleaf;
using Winleafs.Wpf.Api.Effects.ScreenMirrorEffects;
using Winleafs.Wpf.Api.Layouts;
using Winleafs.Wpf.Helpers;

namespace Winleafs.Wpf.Api.Effects
{
    public class ScreenMirrorEffect : ICustomEffect
    {


        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly INanoleafClient _nanoleafClient;
        private readonly Device _device;
        private readonly PanelLayout _panelLayout;
        private readonly System.Timers.Timer _timer;
        private readonly ScreenMirrorAlgorithm _screenMirrorAlgorithm;
        private readonly IScreenMirrorEffect _screenMirrorEffect;

        public ScreenMirrorEffect(Device device, PanelLayout panelLayout, INanoleafClient nanoleafClient)
        {
            _nanoleafClient = nanoleafClient;
            _device = device;
            _panelLayout = panelLayout;
            _screenMirrorAlgorithm = _device.ScreenMirrorAlgorithm;
            var flipType = FlipTypeHelper.ScreenMirrorFlipToFlipType(_device.ScreenMirrorFlip);

            try
            {
                if (_screenMirrorAlgorithm == ScreenMirrorAlgorithm.ScreenMirrorFit)
                {
                    _screenMirrorEffect = new ScreenMirror(_panelLayout, nanoleafClient, ScaleType.Fit, flipType);
                }
                else if (_screenMirrorAlgorithm == ScreenMirrorAlgorithm.ScreenMirrorStretch)
                {
                    _screenMirrorEffect = new ScreenMirror(_panelLayout, nanoleafClient, ScaleType.Stretch, flipType);
                }
                else if (_screenMirrorAlgorithm == ScreenMirrorAlgorithm.Ambilight)
                {
                    _screenMirrorEffect = new Ambilght(nanoleafClient);
                }
            }
            catch (Exception e)
            {
                // It is possible that the user adjusted his/her screens and therefore we get an error when initializing the effect
                _logger.Error(e, $"Something went wrong initializing the screen mirror effect for device {_device.ToString()}");
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
            Task.Run(ApplyEffect);
        }

        private async Task ApplyEffect()
        {
            if (_screenMirrorEffect == null)
            {
                return;
            }

            try
            {
                await _screenMirrorEffect.ApplyEffect();
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Unexpected error for screen mirror with algorithm {_screenMirrorAlgorithm}");

                //Deactivate on error, we do not want to keep applying screen mirror effects when there is one error.
                await Deactivate();
            }
        }

        /// <inheritdoc />
        public async Task Activate()
        {
            if (_screenMirrorAlgorithm is ScreenMirrorAlgorithm.ScreenMirrorFit or ScreenMirrorAlgorithm.ScreenMirrorStretch)
            {
                //For screen mirror, we need to enable external control
                await _nanoleafClient.ExternalControlEndpoint.PrepareForExternalControl(_panelLayout.DeviceType, _device.IPAddress);
            }

            //Start the screengrabber
            ScreenGrabber.Start();

            _timer.Start();
        }

        /// <summary>
        /// Stops the timer and gives it 1 second to complete. Also stop the screengrabber if no other ambilight effects are active
        /// </summary>
        public async Task Deactivate()
        {
            _timer.Stop();
            Thread.Sleep(1000); //Give the last command the time to complete, 1000 is based on testing and a high value (better safe then sorry)

            // Stop the screen grabber
            ScreenGrabber.Stop();
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
        public List<Color> GetColors()
        {
            return new List<Color> { ICustomEffect.DefaultColor };
        }

        public string GetName()
        {
            return $"{UserSettings.EffectNamePreface}Screen mirror";
        }
    }
}
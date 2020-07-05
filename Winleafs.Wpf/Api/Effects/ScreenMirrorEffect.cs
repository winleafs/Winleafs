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
using Winleafs.Wpf.Helpers;

namespace Winleafs.Wpf.Api.Effects
{
    public class ScreenMirrorEffect : ICustomEffect
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        private readonly INanoleafClient _nanoleafClient;
        private readonly Orchestrator _orchestrator;
        private readonly System.Timers.Timer _timer;
        private readonly ScreenMirrorAlgorithm _screenMirrorAlgorithm;
        private readonly IScreenMirrorEffect _screenMirrorEffect;

        public ScreenMirrorEffect(Device device, Orchestrator orchestrator, INanoleafClient nanoleafClient)
        {
            _nanoleafClient = nanoleafClient;
            _orchestrator = orchestrator;
            _screenMirrorAlgorithm = device.ScreenMirrorAlgorithm;

            try
            {
                if (_screenMirrorAlgorithm == ScreenMirrorAlgorithm.ScreenMirrorFit)
                {
                    _screenMirrorEffect = new ScreenMirror(orchestrator, nanoleafClient, ScaleType.Fit);
                }
                else if (_screenMirrorAlgorithm == ScreenMirrorAlgorithm.ScreenMirrorStretch)
                {
                    _screenMirrorEffect = new ScreenMirror(orchestrator, nanoleafClient, ScaleType.Stretch);
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

            _timer = new System.Timers.Timer(100); //Refresh a panel every 10th of a second (10hz)
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
        }

        /// <inheritdoc />
        public async Task Activate()
        {
            if (_screenMirrorAlgorithm == ScreenMirrorAlgorithm.ScreenMirrorFit || _screenMirrorAlgorithm == ScreenMirrorAlgorithm.ScreenMirrorStretch)
            {
                //For screen mirror, we need to enable external control
                await _nanoleafClient.ExternalControlEndpoint.PrepareForExternalControl(_orchestrator.PanelLayout.DeviceType, _orchestrator.Device.IPAddress);
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
            
            //Check if any other screen mirror effects are active, if not, stop the screen grabber
            if (OrchestratorCollection.CountOrchestratorsWithActiveScreenMirrorEffect() <= 0)
            {
                ScreenGrabber.Stop();
            }           
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
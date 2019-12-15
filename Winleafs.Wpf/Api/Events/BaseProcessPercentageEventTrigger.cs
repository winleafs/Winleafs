using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Media;
using Winleafs.Api;
using Winleafs.Api.Endpoints.Interfaces;
using Winleafs.Models.Enums;
using Winleafs.Models.Models.Layouts;
using Winleafs.Models.Models.Scheduling.Triggers;
using Winleafs.Wpf.Helpers;

namespace Winleafs.Wpf.Api.Events
{
    /// <summary>
    /// Base class for process percentage event triggers
    /// </summary>
    public abstract class BaseProcessPercentageEventTrigger : IEventTrigger
    {
        private readonly ITrigger _trigger;
        private readonly Timer _effectTimer;
        private readonly Orchestrator _orchestrator;
        private readonly string _processName;
        private readonly IExternalControlEndpoint _externalControlEndpoint;
        private readonly PercentageProfile _percentageProfile;
        private readonly float _percentagePerStep;
        private readonly int _amountOfSteps;
        private readonly SolidColorBrush _whiteColor = Brushes.White;
        private readonly SolidColorBrush _redColor = Brushes.DarkRed;

        protected BaseProcessPercentageEventTrigger(ITrigger trigger, Orchestrator orchestrator, string processName)
        {
            _trigger = trigger;
            _orchestrator = orchestrator;
            _processName = processName;

            var client = NanoleafClient.GetClientForDevice(_orchestrator.Device);
            _externalControlEndpoint = client.ExternalControlEndpoint;

            //Check if the user has somehow messed up their percentage profile, then we create a single step percentage profile
            if (_orchestrator.Device.PercentageProfile == null || _orchestrator.Device.PercentageProfile.Steps.Count == 0)
            {
                _percentageProfile = new PercentageProfile();
                var step = new PercentageStep();

                foreach (var panel in client.LayoutEndpoint.GetLayout().PanelPositions)
                {
                    step.PanelIds.Add(panel.PanelId);
                }

                _percentageProfile.Steps.Add(step);
                _percentagePerStep = 100f;
                _amountOfSteps = 1;
            }
            else
            {
                _percentageProfile = _orchestrator.Device.PercentageProfile;
                _amountOfSteps = _percentageProfile.Steps.Count;
                _percentagePerStep = 100f / _amountOfSteps;
            }            

            var processCheckTimer = new Timer(60000);
            processCheckTimer.Elapsed += CheckProcess;
            processCheckTimer.AutoReset = true;
            processCheckTimer.Start();

            _effectTimer = new Timer(100);
            _effectTimer.Elapsed += ApplyEffect;
            _effectTimer.AutoReset = true;
        }


        /// <inheritdoc />
        public void StopEvent()
        {
            _effectTimer.Stop();
        }

        /// <inheritdoc />
        public bool IsActive()
        {
            return _effectTimer.Enabled;
        }

        /// <inheritdoc />
        public ITrigger GetTrigger()
        {
            return _trigger;
        }

        private void CheckProcess(object source, ElapsedEventArgs e)
        {
            Task.Run(() => CheckProcessAsync());
        }

        private async Task CheckProcessAsync()
        {
            //Check here if a process is running when the timer is not yet running, then execute TryStartEffect()
            if (!_effectTimer.Enabled)
            {
                Process[] processes = Process.GetProcessesByName(_processName);

                if (processes.Length > 0)
                {
                    await TryStartEffect();
                }
            }            
        }

        private async Task TryStartEffect()
        {
            if (await _orchestrator.TrySetOperationMode(OperationMode.Event))
            {
                await _externalControlEndpoint.PrepareForExternalControl();
                _effectTimer.Start();
            }
        }

        /// <summary>
        /// Applies the effect of the implemented class using the memory reader. If the process quit, the event is stopped
        /// </summary>
        private void ApplyEffect(object source, ElapsedEventArgs e)
        {
            try
            {
                using (var memoryReader = new MemoryReader(_processName))
                {
                    Task.Run(() => ApplyEffectLocalAsync(memoryReader));
                }
            }
            catch
            {
                //Stop the event if the process does not exist anymore
                StopEvent();

                //Let orchestrator know that the process event has stopped so it can continue with normal program, will not fail since an event can only be activated when no override is active
                //Always return to schedule since only 1 event can be active at a time
                Task.Run(() => _orchestrator.TrySetOperationMode(OperationMode.Schedule));
            }
        }

        protected abstract Task ApplyEffectLocalAsync(MemoryReader memoryReader);

        /// <summary>
        /// Sets the percentage according to the defined steps in the percentage profile
        /// </summary>
        protected async Task ApplyPercentageEffect(float percentage)
        {
            var numberOfActiveSteps = _amountOfSteps; //Default the percentage is deemed 100
            if (!float.IsNaN(percentage))
            {
                Math.Max(0, (int)Math.Floor(percentage / _percentagePerStep));
            }            
            
            var activeSteps = _percentageProfile.Steps.Take(numberOfActiveSteps);
            var inactiveSteps = _percentageProfile.Steps.Except(activeSteps);

            foreach (var step in activeSteps)
            {
                foreach (var panel in step.PanelIds)
                {
                    _externalControlEndpoint.SetPanelColorAsync(panel, _redColor.Color.R, _redColor.Color.G, _redColor.Color.B);
                }
            }

            foreach (var step in inactiveSteps)
            {
                foreach (var panel in step.PanelIds)
                {
                    _externalControlEndpoint.SetPanelColorAsync(panel, _whiteColor.Color.R, _whiteColor.Color.G, _whiteColor.Color.B);
                }
            }
        }
    }
}

using System;
using System.Threading.Tasks;
using System.Timers;
using Winleafs.Models.Enums;

namespace Winleafs.Wpf.Api.Events
{
    /// <summary>
    /// Class to test orchestrator
    /// </summary>
    public class TestProcessEvent : IEvent
    {
        private readonly Timer _effectTimer;
        private readonly Orchestrator _orchestrator;

        public TestProcessEvent(Orchestrator orchestrator)
        {
            _orchestrator = orchestrator;

            var processCheckTimer = new Timer(60000);
            processCheckTimer.Elapsed += CheckProcess;
            processCheckTimer.AutoReset = true;
            processCheckTimer.Enabled = true;
            processCheckTimer.Start();

            _effectTimer = new Timer(100);
            _effectTimer.Elapsed += ApplyEffect;
            _effectTimer.AutoReset = true;
            _effectTimer.Enabled = true;
        }

        private void CheckProcess(object source, ElapsedEventArgs e)
        {
            Task.Run(() => CheckProcessAsync());
        }

        private async Task CheckProcessAsync()
        {
            // Check here if a process is running, then execute TryStartEffect(), else stop the effect timer
            var processRunning = false;

#pragma warning disable S2583 // Conditionally executed blocks should be reachable
            if (processRunning)
#pragma warning restore S2583 // Conditionally executed blocks should be reachable
            {
                await TryStartEffect();
            }
            else
            {
                _effectTimer.Stop();
            }
        }

        private async Task TryStartEffect()
        {
            if (await _orchestrator.TrySetOperationMode(OperationMode.Event))
            {
                _effectTimer.Start();
            }
        }

        private void ApplyEffect(object source, ElapsedEventArgs e)
        {
            Task.Run(() => ApplyEffectAsync());
        }

        private async Task ApplyEffectAsync()
        {
            throw new NotSupportedException();
        }

        public void StopEffect()
        {
            _effectTimer.Stop();
        }
    }
}

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
        private Timer _processCheckTimer;
        private Timer _effectTimer;
        private Orchestrator _orchestrator;

        public TestProcessEvent(Orchestrator orchestrator)
        {
            _orchestrator = orchestrator;

            _processCheckTimer = new Timer(60000);
            _processCheckTimer.Elapsed += CheckProcess;
            _processCheckTimer.AutoReset = true;
            _processCheckTimer.Enabled = true;
            _processCheckTimer.Start();

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

        }

        public void StopEffect()
        {
            _effectTimer.Stop();
        }
    }
}

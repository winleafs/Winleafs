using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using Winleafs.Models.Enums;
using Winleafs.Wpf.Helpers;

namespace Winleafs.Wpf.Api.Events
{
    /// <summary>
    /// Base class for process percentage events
    /// </summary>
    public abstract class BaseProcessPercentageEvent : IEvent
    {
        private Timer _processCheckTimer;
        private Timer _effectTimer;
        private Orchestrator _orchestrator;
        private string _processName;

        public BaseProcessPercentageEvent(Orchestrator orchestrator, string processName)
        {
            _orchestrator = orchestrator;
            _processName = processName;

            _processCheckTimer = new Timer(60000);
            _processCheckTimer.Elapsed += CheckProcess;
            _processCheckTimer.AutoReset = true;
            _processCheckTimer.Start();

            _effectTimer = new Timer(100);
            _effectTimer.Elapsed += ApplyEffect;
            _effectTimer.AutoReset = true;
        }

        private void CheckProcess(object source, ElapsedEventArgs e)
        {
            Task.Run(() => CheckProcessAsync());
        }

        private async Task CheckProcessAsync()
        {
            //Check here if a process is running when the timer is not yet running, then execute TryStartEffect(), else stop the effect timer
            if (!_effectTimer.Enabled)
            {
                Process[] processes = Process.GetProcessesByName(_processName);

                if (processes.Length > 0)
                {
                    await TryStartEffect();
                }
                else
                {
                    _effectTimer.Stop();
                }
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
            using (var memoryReader = new MemoryReader(_processName))
            {
                if (memoryReader != null)
                {
                    Task.Run(() => ApplyEffectLocalAsync(memoryReader));
                }
                else
                {
                    StopEffect(); //TODO: let orchestrator know that the effect has stopped so it can continue with normal program
                }
            }
        }

        protected abstract Task ApplyEffectLocalAsync(MemoryReader memoryReader);

        public void StopEffect()
        {
            _effectTimer.Stop();
        }
    }
}

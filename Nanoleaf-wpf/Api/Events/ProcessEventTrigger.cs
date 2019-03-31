using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using Winleafs.Models.Enums;
using Winleafs.Models.Models.Scheduling.Triggers;

namespace Winleafs.Wpf.Api.Events
{
    public class ProcessEventTrigger : IEventTrigger
    {
        private ITrigger _trigger;
        private Timer _processCheckTimer;
        private Orchestrator _orchestrator;
        private string _processName;
        private string _effectName;
        private int _brightness;
        private bool _isActive;

        public ProcessEventTrigger(ITrigger trigger, Orchestrator orchestrator, string processName, string effectName, int brightness)
        {
            _trigger = trigger;
            _orchestrator = orchestrator;
            _processName = processName;
            _effectName = effectName;
            _brightness = brightness;
            _isActive = false;

            _processCheckTimer = new Timer(60000);
            _processCheckTimer.Elapsed += CheckProcess;
            _processCheckTimer.AutoReset = true;
            _processCheckTimer.Start();
        }

        private void CheckProcess(object source, ElapsedEventArgs e)
        {
            Task.Run(() => CheckProcessAsync());
        }

        private async Task CheckProcessAsync()
        {
            //Check here if a process is running then execute TryStartEffect(), else stop the effect
            Process[] processes = Process.GetProcessesByName(_processName);

            if (processes.Length > 0)
            {
                await TryStartEffect();
            }
            else
            {
                if (_isActive)
                {
                    //Let orchestrator know that the process event has stopped so it can continue with normal program, will not fail since an event can only be activated when no override is active
                    //Always return to schedule since only 1 event can be active at a time
                    await _orchestrator.TrySetOperationMode(OperationMode.Schedule);
                    _isActive = false;
                }
            }
        }

        private async Task TryStartEffect()
        {
            if (await _orchestrator.TrySetOperationMode(OperationMode.Event))
            {
                await _orchestrator.ActivateEffect(_effectName, _brightness);
                _isActive = true;
            }
        }

        public void StopEvent()
        {
            _isActive = false;
        }

        public bool IsActive()
        {
            return _isActive;
        }

        public ITrigger GetTrigger()
        {
            return _trigger;
        }
    }
}

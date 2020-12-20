using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using Winleafs.Models.Models;

namespace Winleafs.Wpf.Api.Events
{
    /// <summary>
    /// Event trigger that is activated when a certain process starts
    /// </summary>
    public class ProcessEventTrigger : EventTriggerBase
    {
        private readonly EventTriggersCollection _eventTriggersCollection;
        private readonly string _processName;
        private readonly Timer _processCheckTimer;

        public ProcessEventTrigger(EventTriggersCollection eventTriggersCollection, Models.Models.Scheduling.Triggers.ProcessEventTrigger processEventTrigger)
            : base(processEventTrigger)
        {
            _eventTriggersCollection = eventTriggersCollection;
            _processName = processEventTrigger.ProcessName;

            _processCheckTimer = new Timer(UserSettings.Settings.ProcessResetIntervalInSeconds * 1000);
            _processCheckTimer.Elapsed += CheckProcess;
            _processCheckTimer.AutoReset = true;
            _processCheckTimer.Start();
        }

        public override void Stop()
        {
            _processCheckTimer.Stop();
        }

        private void CheckProcess(object source, ElapsedEventArgs e)
        {
            Task.Run(() => CheckProcessAsync());
        }

        /// <summary>
        /// Checks if a process is running then activate the trigger,
        /// otherwise deactivate the trigger.
        /// </summary>
        private async Task CheckProcessAsync()
        {
            if (TimeIsActive() && Process.GetProcessesByName(_processName).Length > 0)
            {
                await _eventTriggersCollection.ActivateTrigger(Priority);
            }
            else
            {
                await _eventTriggersCollection.DeactivateTrigger(Priority);
            }
        }
    }
}

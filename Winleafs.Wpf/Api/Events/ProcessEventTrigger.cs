using NLog;
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

        public ProcessEventTrigger(EventTriggersCollection eventTriggersCollection, Models.Models.Scheduling.Triggers.ProcessEventTrigger processEventTrigger)
            : base(processEventTrigger.Brightness, processEventTrigger.EffectName, processEventTrigger.Priority)
        {
            _eventTriggersCollection = eventTriggersCollection;
            _processName = processEventTrigger.ProcessName;

            var processCheckTimer = new Timer(UserSettings.Settings.ProcessResetIntervalInSeconds * 1000);
            processCheckTimer.Elapsed += CheckProcess;
            processCheckTimer.AutoReset = true;
            processCheckTimer.Start();
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
            if (Process.GetProcessesByName(_processName).Length > 0)
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

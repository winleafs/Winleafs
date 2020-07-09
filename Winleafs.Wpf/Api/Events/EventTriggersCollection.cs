using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Winleafs.Models.Enums;
using Winleafs.Models.Models;

namespace Winleafs.Wpf.Api.Events
{
    /// <summary>
    /// Collection of event triggers for an orchestrator.
    /// </summary>
    public class EventTriggersCollection
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private List<EventTriggerBase> _eventTriggers;

        //We can use the priority of an event trigger as unique identifier here, since no events can have equal priority
        private HashSet<int> _activeTriggersPriorities;

        public EventTriggerBase ActiveTrigger { get; private set; }

        private readonly Orchestrator _orchestrator;

        public EventTriggersCollection(Orchestrator orchestrator)
        {
            _eventTriggers = new List<EventTriggerBase>();
            _activeTriggersPriorities = new HashSet<int>();
            _orchestrator = orchestrator;

            if (UserSettings.Settings.ActiveSchedule != null && UserSettings.Settings.ActiveSchedule.AppliesToDeviceNames.Contains(orchestrator.Device.Name))
            {
                foreach (var eventTrigger in UserSettings.Settings.ActiveSchedule.EventTriggers)
                {
                    switch (eventTrigger.GetTriggerType())
                    {
                        case TriggerType.ProcessEvent:
                            var processEventTrigger = (Models.Models.Scheduling.Triggers.ProcessEventTrigger)eventTrigger;
                            _eventTriggers.Add(new ProcessEventTrigger(this, processEventTrigger));
                            break;

                        case TriggerType.SpotifyEvent:
                            var spotifyEventTrigger = (Models.Models.Scheduling.Triggers.SpotifyEventTrigger)eventTrigger;
                            _eventTriggers.Add(new SpotifyEventTrigger(this, spotifyEventTrigger));
                            break;
                        
                        //Currently not in use
                        /*case TriggerType.Borderlands2HealthEvent:
                            //This will never be reached currently, since users cannot add this type of event yet
                            EventTriggers.Add(new Borderlands2HealthEventTrigger(eventTrigger, orchestrator));
                            break;*/
                    }
                }
            }
        }

        public async Task ActivateTrigger(int priority)
        {
            if (!_activeTriggersPriorities.Contains(priority))
            {
                _activeTriggersPriorities.Add(priority);
            }

            await SetOperationMode();
        }

        public async Task DeactivateTrigger(int priority)
        {
            if (_activeTriggersPriorities.Remove(priority))
            {
                await SetOperationMode();
            }
        }

        private async Task SetOperationMode()
        {
            if (_activeTriggersPriorities.Any())
            {
                var lowestPriority = _activeTriggersPriorities.Min();
                if (ActiveTrigger == null || ActiveTrigger.Priority != lowestPriority)
                {
                    //Event operation mode is already active or we try to activate it (can fail if the user has manual enabled)
                    var eventOperationModeActive = _orchestrator.Device.OperationMode == OperationMode.Event || await _orchestrator.TrySetOperationMode(OperationMode.Event);

                    //A trigger has been activated or deactivated and the current active trigger is no longer the trigger with the highest priority (priority 1 is highest)
                    ActiveTrigger = _eventTriggers.FirstOrDefault(eventTrigger => eventTrigger.Priority == lowestPriority);

                    if (eventOperationModeActive)
                    {
                        await _orchestrator.ActivateEffect(ActiveTrigger.EffectName, ActiveTrigger.Brightness);

                        _logger.Info($"Process event started with effect {ActiveTrigger.EffectName} with brightness {ActiveTrigger.Brightness} for device {_orchestrator.Device.IPAddress}");
                    }
                }
            }
            else if (_orchestrator.Device.OperationMode == OperationMode.Event) //Only go back to schedule if the current devices operation mode is event
            {
                //Let orchestrator know that all events have stopped so it can continue with normal program, will not fail since an event can only be activated when no override is active
                //Always return to schedule since only 1 event can be active at a time
                ActiveTrigger = null;
                await _orchestrator.TrySetOperationMode(OperationMode.Schedule);

                _logger.Info($"Stopped all process events for device {_orchestrator.Device.IPAddress}");
            }
        }

        public void StopAllEvents()
        {
            ActiveTrigger = null;
            _activeTriggersPriorities.Clear();
        }
    }
}

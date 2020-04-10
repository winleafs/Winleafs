using System.Collections.Generic;
using Winleafs.Models.Enums;
using Winleafs.Models.Models;

namespace Winleafs.Wpf.Api.Events
{
    /// <summary>
    /// Collection of event triggers for an orchestrator.
    /// </summary>
    public class EventTriggersCollection
    {
        public List<IEventTrigger> EventTriggers { get; set; }

        public EventTriggersCollection(Orchestrator orchestrator)
        {
            EventTriggers = new List<IEventTrigger>();

            if (UserSettings.Settings.ActiveSchedule != null && UserSettings.Settings.ActiveSchedule.AppliesToDeviceNames.Contains(orchestrator.Device.Name))
            {
                foreach (var eventTrigger in UserSettings.Settings.ActiveSchedule.EventTriggers)
                {
                    switch (eventTrigger.GetTriggerType())
                    {
                        case TriggerType.ProcessEvent:
                            var processEventTrigger = (Models.Models.Scheduling.Triggers.ProcessEventTrigger)eventTrigger;
                            EventTriggers.Add(new ProcessEventTrigger(eventTrigger, orchestrator, processEventTrigger.ProcessName, processEventTrigger.EffectName, processEventTrigger.Brightness));
                            break;

                        case TriggerType.SpotifyEvent:
                            var spotifyEventTrigger = (Models.Models.Scheduling.Triggers.SpotifyEventTrigger)eventTrigger;
                            EventTriggers.Add(new SpotifyEventTrigger(eventTrigger, orchestrator, spotifyEventTrigger.PlaylistId, spotifyEventTrigger.EffectName, spotifyEventTrigger.Brightness));
                            break;

                        case TriggerType.Borderlands2HealthEvent:
                            //This will never be reached currently, since users cannot add this type of event yet
                            EventTriggers.Add(new Borderlands2HealthEventTrigger(eventTrigger, orchestrator));
                            break;
                    }
                }
            }            
            
        }

        public void StopAllEvents()
        {
            foreach (var eventTrigger in EventTriggers)
            {
                eventTrigger.StopEvent();
            }
        }
    }
}

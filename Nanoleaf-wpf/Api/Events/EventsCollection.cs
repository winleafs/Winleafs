using System.Collections.Generic;
using Winleafs.Models.Enums;
using Winleafs.Models.Models.Scheduling.Triggers;

namespace Winleafs.Wpf.Api.Events
{
    public class EventsCollection
    {
        public List<IEvent> Events { get; set; }

        public EventsCollection(Orchestrator orchestrator)
        {
            Events = new List<IEvent>();

            if (orchestrator.Device.ActiveSchedule != null)
            {
                foreach (var eventTrigger in orchestrator.Device.ActiveSchedule.EventTriggers)
                {
                    switch (eventTrigger.GetTriggerType())
                    {
                        case TriggerType.ProcessEvent:
                            var processEventTrigger = (ProcessEventTrigger)eventTrigger;
                            Events.Add(new ProcessEvent(orchestrator, processEventTrigger.ProcessName, processEventTrigger.EffectName, processEventTrigger.Brightness));
                            break;

                        case TriggerType.Borderlands2HealthEvent:
                            //This will never be reached currently, since users cannot add this type of event yet
                            Events.Add(new Borderlands2HealthEvent(orchestrator));
                            break;
                    }
                }
            }            
            
        }

        public void StopAllEvents()
        {
            foreach (var ievent in Events)
            {
                ievent.StopEvent();
            }
        }
    }
}

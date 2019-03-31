using System.Collections.Generic;
using Winleafs.Models.Enums;

namespace Winleafs.Wpf.Api.Events
{
    public class EventTriggersCollection
    {
        public List<IEventTrigger> Events { get; set; }

        public EventTriggersCollection(Orchestrator orchestrator)
        {
            Events = new List<IEventTrigger>();

            if (orchestrator.Device.ActiveSchedule != null)
            {
                foreach (var eventTrigger in orchestrator.Device.ActiveSchedule.EventTriggers)
                {
                    switch (eventTrigger.GetTriggerType())
                    {
                        case TriggerType.ProcessEvent:
                            var processEventTrigger = (Models.Models.Scheduling.Triggers.ProcessEventTrigger)eventTrigger;
                            Events.Add(new ProcessEventTrigger(eventTrigger, orchestrator, processEventTrigger.ProcessName, processEventTrigger.EffectName, processEventTrigger.Brightness));
                            break;

                        case TriggerType.Borderlands2HealthEvent:
                            //This will never be reached currently, since users cannot add this type of event yet
                            Events.Add(new Borderlands2HealthEventTrigger(eventTrigger, orchestrator));
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

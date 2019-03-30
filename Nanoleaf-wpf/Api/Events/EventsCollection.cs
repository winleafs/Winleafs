using System.Collections.Generic;
using Winleafs.Models.Enums;

namespace Winleafs.Wpf.Api.Events
{
    public class EventsCollection
    {
        private List<IEvent> _events;

        public EventsCollection(Orchestrator orchestrator)
        {
            _events = new List<IEvent>();

            foreach (var processEvent in orchestrator.Device.ProcessEvents)
            {
                switch (processEvent.ProcessEventType)
                {
                    case ProcessEventType.Process:
                        _events.Add(new ProcessEvent(orchestrator, processEvent.ProcessName, processEvent.EffectName, processEvent.Brightness));
                        break;

                    case ProcessEventType.Borderlands2Health:
                        //This will never be reached currently, since users cannot add this type of event yet
                        _events.Add(new Borderlands2HealthEvent(orchestrator));
                        break;
                }
            }
            
        }

        public void StopAllEvents()
        {
            foreach (var ievent in _events)
            {
                ievent.StopEvent();
            }
        }
    }
}

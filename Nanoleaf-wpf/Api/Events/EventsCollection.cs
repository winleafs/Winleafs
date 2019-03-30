using System.Collections.Generic;

namespace Winleafs.Wpf.Api.Events
{
    public class EventsCollection
    {
        private Dictionary<string, IEvent> _events;

        public EventsCollection(Orchestrator orchestrator)
        {
            _events = new Dictionary<string, IEvent>();

            //TODO: only enable this effect if the user added it in their schedule
            _events.Add(Borderlands2HealthEvent.EventName, new Borderlands2HealthEvent(orchestrator));
        }

        public void StopAllEvents()
        {
            foreach (var ievent in _events.Values)
            {
                ievent.StopEvent();
            }
        }
    }
}

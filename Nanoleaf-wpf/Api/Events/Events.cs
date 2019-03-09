using System.Collections.Generic;

namespace Winleafs.Wpf.Api.Events
{
    public class Events
    {
        private Dictionary<string, IEvent> _events;

        public Events(Orchestrator orchestrator)
        {
            _events = new Dictionary<string, IEvent>();

            _events.Add("TestProcessEvent", new TestProcessEvent(orchestrator));
        }

        public void StopAllEvents()
        {
            foreach (var ievent in _events)
            {
                ievent.Value.StopEffect();
            }
        }
    }
}

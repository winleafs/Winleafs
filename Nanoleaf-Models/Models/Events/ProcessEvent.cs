using Winleafs.Models.Enums;

namespace Winleafs.Models.Models.Events
{
    public class ProcessEvent : BaseEvent
    {
        public string ProcessName { get; set; }

        public ProcessEventType ProcessEventType { get; set; }
    }
}

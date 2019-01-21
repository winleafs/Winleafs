using Nanoleaf_Models.Enums;
using Nanoleaf_Models.Models.Scheduling.Triggers;
using System.Collections.Generic;
using System.Linq;

namespace Nanoleaf_Models.Models.Scheduling
{
    public class Program
    {
        public List<TimeTrigger> Triggers { get; set; }

        public Program()
        {
            Triggers = new List<TimeTrigger>();
        }

        public void AddTrigger(TriggerType triggerType, int hours, int minutes, string effect, int brightness)
        {
            Triggers.Add(new TimeTrigger
            {
                Effect = effect,
                Minutes = minutes,
                Hours = hours,
                TriggerType = triggerType,
                Brightness = brightness
            });

            Triggers = Triggers.OrderBy(t => t.Hours).ThenBy(t => t.Minutes).ToList();
        }
    }
}

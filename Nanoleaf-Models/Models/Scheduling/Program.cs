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

        public void AddTrigger(TimeTrigger trigger)
        {
            Triggers.Add(trigger);
            Triggers = Triggers.OrderBy(t => t.Hours).ThenBy(t => t.Minutes).ToList();
        }
    }
}

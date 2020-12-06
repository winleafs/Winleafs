using System;
using System.Collections.Generic;
using System.Linq;
using Winleafs.Models.Models.Scheduling.Triggers;

namespace Winleafs.Models.Models.Scheduling
{
    public class Program
    {
        public List<ScheduleTrigger> Triggers { get; set; }

        public Program()
        {
            Triggers = new List<ScheduleTrigger>();
        }

        public void AddTrigger(ScheduleTrigger trigger)
        {
            Triggers.Add(trigger);
            ReorderTriggers();
        }

        /// <summary>
        /// Returns the TimeTrigger that should be active at this moment
        /// </summary>
        public void ReorderTriggers()
        {
            var now = DateTime.Now;
            Triggers = Triggers.OrderBy(t => t.TimeComponent.GetActualDateTime(now)).ToList();
        }

        /// <summary>
        /// Checks whether a trigger overlaps any existing triggers timewise. Using nested ifs for readability
        /// </summary>
        public bool TriggerOverlaps(ScheduleTrigger otherTrigger)
        {
            foreach (var trigger in Triggers)
            {
                if (trigger.TimeComponent.Overlaps(otherTrigger.TimeComponent))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

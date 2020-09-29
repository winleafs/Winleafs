using System;
using System.Collections.Generic;
using System.Linq;
using Winleafs.Models.Enums;
using Winleafs.Models.Models.Scheduling.Triggers;

namespace Winleafs.Models.Models.Scheduling
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
            ReorderTriggers();
        }

        /// <summary>
        /// Returns the TimeTrigger that should be active at this moment
        /// </summary>
        public void ReorderTriggers()
        {
            var now = DateTime.Now;
            Triggers = Triggers.OrderBy(t => t.GetActualDateTime(now)).ToList();
        }

        /// <summary>
        /// Checks whether a trigger overlaps any existing triggers timewise. Using nested ifs for readability
        /// </summary>
        public bool TriggerOverlaps(TimeTrigger otherTrigger)
        {
            foreach (var trigger in Triggers)
            {
                if (trigger.TimeType == TimeType.Sunrise
                    && otherTrigger.TimeType == TimeType.Sunrise
                    && trigger.BeforeAfter == otherTrigger.BeforeAfter
                    && trigger.ExtraHours == otherTrigger.ExtraHours
                    && trigger.ExtraMinutes == otherTrigger.ExtraMinutes)
                {
                    return true;
                }

                if (trigger.TimeType == TimeType.Sunset && otherTrigger.TimeType == TimeType.Sunset
                    && trigger.BeforeAfter == otherTrigger.BeforeAfter
                    && trigger.ExtraHours == otherTrigger.ExtraHours
                    && trigger.ExtraMinutes == otherTrigger.ExtraMinutes)
                {
                    return true;
                }

                if (trigger.TimeType == TimeType.FixedTime
                         && otherTrigger.TimeType == TimeType.FixedTime
                        && trigger.Hours == otherTrigger.Hours
                         && trigger.Minutes == otherTrigger.Minutes)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

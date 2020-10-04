using System;
using System.Collections.Generic;
using System.Linq;
using Winleafs.Models.Enums;
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
            //TODO: MOVE THIS TO A EQUALS METHOD IN TIMECOMPONENT
            foreach (var trigger in Triggers)
            {
                if (trigger.TimeComponent.TimeType == TimeType.Sunrise
                    && otherTrigger.TimeComponent.TimeType == TimeType.Sunrise
                    && trigger.TimeComponent.BeforeAfter == otherTrigger.TimeComponent.BeforeAfter
                    && trigger.TimeComponent.ExtraHours == otherTrigger.TimeComponent.ExtraHours
                    && trigger.TimeComponent.ExtraMinutes == otherTrigger.TimeComponent.ExtraMinutes)
                {
                    return true;
                }

                if (trigger.TimeComponent.TimeType == TimeType.Sunset && otherTrigger.TimeComponent.TimeType == TimeType.Sunset
                    && trigger.TimeComponent.BeforeAfter == otherTrigger.TimeComponent.BeforeAfter
                    && trigger.TimeComponent.ExtraHours == otherTrigger.TimeComponent.ExtraHours
                    && trigger.TimeComponent.ExtraMinutes == otherTrigger.TimeComponent.ExtraMinutes)
                {
                    return true;
                }

                if (trigger.TimeComponent.TimeType == TimeType.FixedTime
                         && otherTrigger.TimeComponent.TimeType == TimeType.FixedTime
                        && trigger.TimeComponent.Hours == otherTrigger.TimeComponent.Hours
                         && trigger.TimeComponent.Minutes == otherTrigger.TimeComponent.Minutes)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

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
            Triggers = Triggers.OrderBy(timeTrigger => timeTrigger.GetActualDateTime(DateTime.Now)).ToList();
        }

        /// <summary>
        /// Checks whether a trigger overlaps any existing triggers timewise. Using nested ifs for readability
        /// </summary>
        public bool TriggerOverlaps(TimeTrigger otherTrigger)
        {
            return Triggers.Any(trigger =>
                IsEqualOnSunrise(trigger, otherTrigger)
                || IsEqualOnSunset(trigger, otherTrigger)
                || IsEqualOnTime(trigger, otherTrigger));
        }

        private static bool IsEqualOnSunrise(TimeTrigger trigger, TimeTrigger otherTrigger)
        {
            return trigger.EventTriggerType == TriggerType.Sunrise
                   && otherTrigger.EventTriggerType == TriggerType.Sunrise
                   && trigger.BeforeAfter == otherTrigger.BeforeAfter
                   && trigger.ExtraHours == otherTrigger.ExtraHours
                   && trigger.ExtraMinutes == otherTrigger.ExtraMinutes;
        }

        private static bool IsEqualOnSunset(TimeTrigger trigger, TimeTrigger otherTrigger)
        {
            return trigger.EventTriggerType == TriggerType.Sunset
                   && otherTrigger.EventTriggerType == TriggerType.Sunset
                   && trigger.BeforeAfter == otherTrigger.BeforeAfter
                   && trigger.ExtraHours == otherTrigger.ExtraHours
                   && trigger.ExtraMinutes == otherTrigger.ExtraMinutes;
        }

        private static bool IsEqualOnTime(TimeTrigger trigger, TimeTrigger otherTrigger)
        {
            return trigger.EventTriggerType == TriggerType.Time
                   && otherTrigger.EventTriggerType == TriggerType.Time
                   && trigger.Hours == otherTrigger.Hours
                   && trigger.Minutes == otherTrigger.Minutes;
        }
    }
}

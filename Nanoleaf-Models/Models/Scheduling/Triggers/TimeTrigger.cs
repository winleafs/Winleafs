using Nanoleaf_Models.Enums;
using System;

namespace Nanoleaf_Models.Models.Scheduling.Triggers
{
    public class TimeTrigger : ITrigger
    {
        public int Hours { get; set; }
        public int Minutes { get; set; }

        public TriggerType TriggerType { get; set; }

        public string Effect { get; set; }

        public int Brightness { get; set; }

        public BeforeAfter BeforeAfter { get; set; }
        public int ExtraHours { get; set; }
        public int ExtraMinutes { get; set; }

        public string GetDisplayName()
        {
            if (TriggerType == TriggerType.Time)
            {
                var hours = Hours < 10 ? $"0{Hours}" : Hours.ToString();
                var minutes = Minutes < 10 ? $"0{Minutes}" : Minutes.ToString();
                return $"{hours}:{minutes}";
            }
            else
            {
                return TriggerType.ToString();
            }
        }

        public string GetDescription()
        {
            return Effect;
        }

        public TriggerType GetTriggerType()
        {
            return TriggerType;
        }

        /// <summary>
        /// Returns a date time object for the actual time of this trigger
        /// </summary>
        public DateTime GetActualDateTime(DateTime dateOfProgram)
        {
            var date = new DateTime(dateOfProgram.Year, dateOfProgram.Month, dateOfProgram.Day, Hours, Minutes, 0);

            if (BeforeAfter == BeforeAfter.After)
            {
                date.AddHours(ExtraHours);
                date.AddMinutes(ExtraMinutes);
            }
            else if (BeforeAfter == BeforeAfter.Before)
            {
                date.AddHours(-ExtraHours);
                date.AddMinutes(-ExtraMinutes);
            }

            return date;
        }
    }
}

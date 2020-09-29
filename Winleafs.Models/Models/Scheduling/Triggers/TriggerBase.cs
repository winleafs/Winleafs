using System;
using Winleafs.Models.Enums;

namespace Winleafs.Models.Models.Scheduling.Triggers
{
    /// <summary>
    /// Base class for all triggers
    /// </summary>
    public abstract class TriggerBase
    {
        public string EffectName { get; set; }
        public int Brightness { get; set; }
        public TimeType TimeType { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public BeforeAfter BeforeAfter { get; set; }
        public int ExtraHours { get; set; }
        public int ExtraMinutes { get; set; }

        public virtual string GetDescription()
        {
            return EffectName;
        }

        /// <summary>
        /// Returns a DateTime object for the actual time of this trigger, with the year, month and day set to the current date
        /// </summary>
        public DateTime GetActualDateTime()
        {
            return GetActualDateTime(DateTime.Now);
        }

        /// <summary>
        /// Returns a DateTime object for the actual time of this trigger
        /// </summary>
        public DateTime GetActualDateTime(DateTime dateOfProgram)
        {
            var date = new DateTime(dateOfProgram.Year, dateOfProgram.Month, dateOfProgram.Day, Hours, Minutes, 0);

            if (BeforeAfter == BeforeAfter.After)
            {
                date = date.AddHours(ExtraHours);
                date = date.AddMinutes(ExtraMinutes);
            }
            else if (BeforeAfter == BeforeAfter.Before)
            {
                date = date.AddHours(-ExtraHours);
                date = date.AddMinutes(-ExtraMinutes);
            }

            return date;
        }
    }
}

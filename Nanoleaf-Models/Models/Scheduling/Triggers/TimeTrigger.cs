using System;

using Winleafs.Models.Enums;

namespace Winleafs.Models.Models.Scheduling.Triggers
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

        public string GetDescription()
        {
            if (ExtraHours > 0 || ExtraMinutes > 0)
            {
                var extraHours = ExtraHours < 10 ? $"0{ExtraHours}" : ExtraHours.ToString();
                var extraMinutes = ExtraMinutes < 10 ? $"0{ExtraMinutes}" : ExtraMinutes.ToString();
                return $"{TriggerType.ToString()} ({(BeforeAfter == BeforeAfter.Before ? "-" : "+")} {extraHours}:{extraMinutes}): {GetActualDateTime().ToString("HH:mm")}";
            }
            else
            {
                return $"{TriggerType.ToString()}: {GetActualDateTime().ToString("HH:mm")}";
            }
        }

        public TriggerType GetTriggerType()
        {
            return TriggerType;
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

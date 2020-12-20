using System;
using Winleafs.Models.Enums;

namespace Winleafs.Models.Models.Scheduling.Triggers
{
    public class TimeComponent
    {
        public TimeType TimeType { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public BeforeAfter BeforeAfter { get; set; }
        public int ExtraHours { get; set; }
        public int ExtraMinutes { get; set; }

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

        public void UpdateSunTimes(int sunriseHour, int sunriseMinute, int sunsetHour, int sunsetMinute)
        {
            switch (TimeType)
            {
                case TimeType.Sunrise:
                    Hours = sunriseHour;
                    Minutes = sunriseMinute;
                    break;
                case TimeType.Sunset:
                    Hours = sunsetHour;
                    Minutes = sunsetMinute;
                    break;
            }
        }

        public bool Overlaps(TimeComponent otherTimeComponent)
        {
            if (TimeType == TimeType.Sunrise && otherTimeComponent.TimeType == TimeType.Sunrise
                && BeforeAfter == otherTimeComponent.BeforeAfter
                && ExtraHours == otherTimeComponent.ExtraHours
                && ExtraMinutes == otherTimeComponent.ExtraMinutes)
            {
                return true;
            }

            if (TimeType == TimeType.Sunset && otherTimeComponent.TimeType == TimeType.Sunset
                && BeforeAfter == otherTimeComponent.BeforeAfter
                && ExtraHours == otherTimeComponent.ExtraHours
                && ExtraMinutes == otherTimeComponent.ExtraMinutes)
            {
                return true;
            }

            if (TimeType == TimeType.FixedTime && otherTimeComponent.TimeType == TimeType.FixedTime
                && Hours == otherTimeComponent.Hours
                && Minutes == otherTimeComponent.Minutes)
            {
                return true;
            }

            return false;
        }
    }
}
